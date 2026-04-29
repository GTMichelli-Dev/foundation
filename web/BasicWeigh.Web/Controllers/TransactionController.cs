using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Hubs;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Services;

namespace BasicWeigh.Web.Controllers;

public class TransactionController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly IScaleService _scaleService;
    private readonly PrintQueueService _printQueue;
    private readonly IHubContext<ScaleHub> _hub;
    private readonly AppSetupCache _setupCache;
    private readonly ILogger<TransactionController> _log;

    public TransactionController(ScaleDbContext db, IScaleService scaleService,
        PrintQueueService printQueue, IHubContext<ScaleHub> hub, AppSetupCache setupCache,
        ILogger<TransactionController> log)
    {
        _db = db;
        _scaleService = scaleService;
        _printQueue = printQueue;
        _hub = hub;
        _setupCache = setupCache;
        _log = log;
    }

    private void PopulateDropdowns()
    {
        ViewBag.Customers = _db.Customers.Where(c => c.Active).OrderBy(c => c.CustomerName).ToList();

        // Carriers = dedicated carriers + all active customers (any customer can be a carrier)
        var carrierNames = _db.Carriers.Where(c => c.Active).Select(c => c.CarrierName).ToList();
        var customerNames = _db.Customers.Where(c => c.Active).Select(c => c.CustomerName).ToList();
        ViewBag.CarrierOptions = carrierNames.Union(customerNames).OrderBy(n => n).ToList();

        // Trucks loaded via AJAX based on selected carrier
        ViewBag.Commodities = _db.Commodities.Where(c => c.Active).OrderBy(c => c.CommodityName).ToList();
        ViewBag.Locations = _db.Locations.Where(l => l.Active).OrderBy(l => l.LocationName).ToList();
        ViewBag.Destinations = _db.Destinations.Where(d => d.Active).OrderBy(d => d.DestinationName).ToList();
    }

    // GET: Transaction/WeighIn (new)
    // GET: Transaction/WeighIn/5 (edit existing inbound)
    public IActionResult WeighIn(string? id)
    {
        PopulateDropdowns();
        ViewBag.CurrentWeight = _scaleService.GetCurrentWeight();

        var setup = _setupCache.Get();
        ViewBag.SavePicture = setup.SavePicture;

        if (!string.IsNullOrEmpty(id))
        {
            var existing = _db.Transactions.Find(id);
            if (existing == null) return NotFound();
            if (existing.DateOut != null) return RedirectToAction("Edit", new { id });

            ViewBag.IsEdit = true;
            if (setup.SavePicture)
            {
                var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tickets");
                ViewBag.HasInPhoto = System.IO.File.Exists(Path.Combine(imgDir, $"{id}_In.jpg"));
            }
            return View(existing);
        }

        ViewBag.NextTicket = setup.TicketNumber.ToString();
        ViewBag.IsEdit = false;

        var txn = new Transaction();
        // Recall last ticket values if enabled
        if (setup.RecallLastValues)
        {
            var lastTxn = _db.Transactions
                .OrderByDescending(t => t.DateIn)
                .FirstOrDefault();
            if (lastTxn != null)
            {
                txn.Commodity = lastTxn.Commodity;
                txn.Customer = lastTxn.Customer;
                txn.Location = lastTxn.Location;
                txn.Destination = lastTxn.Destination;
            }
        }
        return View(txn);
    }

    // POST: Transaction/WeighIn
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WeighIn(Transaction transaction, bool isEdit, bool goToWeighOut, bool manualWeight, bool completeWithRetainedTare = false)
    {
        if (isEdit)
        {
            var existing = _db.Transactions.Find(transaction.Ticket);
            if (existing == null) return NotFound();

            existing.InWeight = transaction.InWeight;
            existing.ManualInbound = manualWeight;
            existing.DateIn = transaction.DateIn;
            existing.Customer = transaction.Customer;
            existing.Carrier = transaction.Carrier;
            existing.TruckId = transaction.TruckId;
            existing.Commodity = transaction.Commodity;
            existing.Location = transaction.Location;
            existing.Destination = transaction.Destination;
            existing.Notes = transaction.Notes;

            _db.SaveChanges();

            if (goToWeighOut)
                return RedirectToAction("WeighOut", new { id = transaction.Ticket });
        }
        else
        {
            var setup = _db.AppSetup.First();
            while (_db.Transactions.Any(t => t.Ticket == setup.TicketNumber.ToString()))
            {
                setup.TicketNumber++;
            }
            transaction.Ticket = setup.TicketNumber.ToString();
            transaction.DateIn = transaction.DateIn == default ? DateTime.UtcNow : transaction.DateIn;
            transaction.Void = false;
            transaction.ManualInbound = manualWeight;

            // Complete-with-retained-tare path. Look up the truck server-side
            // (don't trust the client) and, if the feature is on and a tare is
            // stored, save the ticket as already completed using that tare.
            bool completed = false;
            if (completeWithRetainedTare && setup.UseRetainedTare
                && !string.IsNullOrEmpty(transaction.TruckId)
                && !string.IsNullOrEmpty(transaction.Carrier))
            {
                var tid = transaction.TruckId.Trim().ToLower();
                var car = transaction.Carrier.Trim().ToLower();
                var truck = _db.Trucks.FirstOrDefault(x =>
                    x.TruckId.ToLower() == tid && x.CarrierName.ToLower() == car);

                // Tares from a previous date are auto-expired (gated on
                // AutoClearStaleRetainedTare).
                if (setup.AutoClearStaleRetainedTare
                    && truck?.RetainedTare.HasValue == true
                    && (truck.RetainedTareUpdated?.Date ?? DateTime.MinValue) < DateTime.Today)
                {
                    Console.WriteLine($"[RetainedTare] cleared stale tare for '{truck.TruckId}' / '{truck.CarrierName}' (last seen {truck.RetainedTareUpdated:yyyy-MM-dd})");
                    _log.LogInformation("RetainedTare: cleared stale tare for {TruckId}/{Carrier}", truck.TruckId, truck.CarrierName);
                    truck.RetainedTare = null;
                    truck.RetainedTareUpdated = null;
                }

                if (truck?.RetainedTare.HasValue == true)
                {
                    transaction.OutWeight = truck.RetainedTare;
                    transaction.DateOut = transaction.DateIn;
                    transaction.ManualOutbound = false;
                    completed = true;
                    var msg = $"WeighIn (admin) ticket {transaction.Ticket}: completed with recalled tare {truck.RetainedTare} lb (truck '{truck.TruckId}' / '{truck.CarrierName}')";
                    _log.LogInformation(msg);
                    Console.WriteLine($"[RetainedTare] {msg}");
                }
            }

            setup.TicketNumber++;
            _db.AppSetup.Update(setup);

            _db.Transactions.Add(transaction);
            _db.SaveChanges();
            _setupCache.Invalidate();

            // A tare-completed ticket is effectively a weigh-out — route the
            // camera capture and the print to the outbound side and use type
            // "weighout" so the print agent picks the right ticket layout.
            // The regular open-ticket flow uses the inbound side below.
            if (completed)
            {
                if (setup.SavePicture && !manualWeight && !string.IsNullOrEmpty(setup.OutboundCameraId))
                {
                    var parts = setup.OutboundCameraId.Split(':', 2);
                    var serviceId = parts.Length > 1 ? parts[0] : "default";
                    var cameraId = parts.Length > 1 ? parts[1] : parts[0];
                    await _hub.Clients.Group($"Camera_{serviceId}").SendAsync("CaptureImage",
                        new { ticket = transaction.Ticket, direction = "out", cameraId });
                }

                var outboundPrinter = setup.OutboundPrinterId;
                if (!string.IsNullOrEmpty(outboundPrinter))
                {
                    if (outboundPrinter.Equals("Browser:Browser", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["AutoPrintTicket"] = transaction.Ticket;
                        TempData["AutoPrintType"] = "weighout";
                    }
                    else
                    {
                        var parts2 = outboundPrinter.Split(':', 2);
                        var svcId = parts2.Length > 1 ? parts2[0] : "";
                        var pName = parts2.Length > 1 ? parts2[1] : parts2[0];
                        var group = !string.IsNullOrEmpty(svcId) ? $"Print_{svcId}" : "PrintClients";
                        await _hub.Clients.Group(group).SendAsync("PrintTicket",
                            new { ticketId = transaction.Ticket, type = "weighout", printerId = pName });
                    }
                }

                // Same destination as the WeighOut page so the operator lands on
                // the ticket they just completed and can print from there even
                // when no printer is configured.
                return RedirectToAction("View", "Ticket", new { id = transaction.Ticket });
            }

            // Camera capture on inbound (only if not manual weight)
            if (setup.SavePicture && !manualWeight && !string.IsNullOrEmpty(setup.InboundCameraId))
            {
                var parts = setup.InboundCameraId.Split(':', 2);
                var serviceId = parts.Length > 1 ? parts[0] : "default";
                var cameraId = parts.Length > 1 ? parts[1] : parts[0];
                await _hub.Clients.Group($"Camera_{serviceId}").SendAsync("CaptureImage",
                    new { ticket = transaction.Ticket, direction = "in", cameraId });
            }

            // Auto-print to configured inbound printer
            var inboundPrinter = setup.InboundPrinterId;
            if (!string.IsNullOrEmpty(inboundPrinter))
            {
                if (inboundPrinter.Equals("Browser:Browser", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["AutoPrintTicket"] = transaction.Ticket;
                    TempData["AutoPrintType"] = "weighin";
                }
                else
                {
                    var parts2 = inboundPrinter.Split(':', 2);
                    var svcId = parts2.Length > 1 ? parts2[0] : "";
                    var pName = parts2.Length > 1 ? parts2[1] : parts2[0];
                    var group = !string.IsNullOrEmpty(svcId) ? $"Print_{svcId}" : "PrintClients";
                    await _hub.Clients.Group(group).SendAsync("PrintTicket",
                        new { ticketId = transaction.Ticket, type = "weighin", printerId = pName });
                }
            }
        }

        return RedirectToAction("InboundTrucks");
    }

    // GET: Transaction/WeighOut/5
    public IActionResult WeighOut(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        ViewBag.CurrentWeight = _scaleService.GetCurrentWeight();
        PopulateDropdowns();
        return View(transaction);
    }

    // POST: Transaction/WeighOut/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WeighOut(string id, Transaction transaction, bool manualOutWeight, bool manualInWeight)
    {
        var existing = _db.Transactions.Find(id);
        if (existing == null) return NotFound();

        existing.InWeight = transaction.InWeight;
        existing.ManualInbound = manualInWeight;
        existing.OutWeight = transaction.OutWeight;
        existing.ManualOutbound = manualOutWeight;
        existing.DateOut = transaction.DateOut ?? DateTime.UtcNow;
        existing.DateIn = transaction.DateIn;
        existing.Customer = transaction.Customer;
        existing.Carrier = transaction.Carrier;
        existing.TruckId = transaction.TruckId;
        existing.Commodity = transaction.Commodity;
        existing.Location = transaction.Location;
        existing.Destination = transaction.Destination;
        existing.Notes = transaction.Notes;

        var setup = _setupCache.Get();
        var weighOutMsg = $"WeighOut (admin) ticket {existing.Ticket}: UseRetainedTare={setup.UseRetainedTare} TruckId='{existing.TruckId}' Carrier='{existing.Carrier}'";
        _log.LogInformation(weighOutMsg);
        Console.WriteLine($"[RetainedTare] {weighOutMsg}");
        if (setup.UseRetainedTare)
        {
            UpdateRetainedTare(existing);
        }

        _db.SaveChanges();

        // Camera capture on outbound (only if not manual weight)
        if (setup.SavePicture && !manualOutWeight && !string.IsNullOrEmpty(setup.OutboundCameraId))
        {
            var parts = setup.OutboundCameraId.Split(':', 2);
            var serviceId = parts.Length > 1 ? parts[0] : "default";
            var cameraId = parts.Length > 1 ? parts[1] : parts[0];
            await _hub.Clients.Group($"Camera_{serviceId}").SendAsync("CaptureImage",
                new { ticket = id, direction = "out", cameraId });
        }

        // Auto-print to configured outbound printer
        var outboundPrinter = setup.OutboundPrinterId;
        if (!string.IsNullOrEmpty(outboundPrinter))
        {
            if (outboundPrinter.Equals("Browser:Browser", StringComparison.OrdinalIgnoreCase))
            {
                // Redirect to ticket view with auto-print — opens in new window via CompletedTrucks
                TempData["AutoPrintTicket"] = id;
            }
            else
            {
                var parts2 = outboundPrinter.Split(':', 2);
                var svcId = parts2.Length > 1 ? parts2[0] : "";
                var pName = parts2.Length > 1 ? parts2[1] : parts2[0];
                var group = !string.IsNullOrEmpty(svcId) ? $"Print_{svcId}" : "PrintClients";
                await _hub.Clients.Group(group).SendAsync("PrintTicket",
                    new { ticketId = id, type = "weighout", printerId = pName });
            }
        }

        return RedirectToAction("View", "Ticket", new { id });
    }

    // GET: Transaction/InboundTrucks
    public IActionResult InboundTrucks()
    {
        var setup = _setupCache.Get();
        ViewBag.RemotePrintMode = setup.RemotePrintMode ?? "None";
        ViewBag.KioskCount = setup.KioskCount;
        ViewBag.DemoMode = setup.DemoMode;
        ViewBag.SavePicture = setup.SavePicture;
        return View();
    }

    // GET: Transaction/CompletedTrucks
    public IActionResult CompletedTrucks()
    {
        var setup = _setupCache.Get();
        ViewBag.RemotePrintMode = setup.RemotePrintMode ?? "None";
        ViewBag.KioskCount = setup.KioskCount;
        ViewBag.DemoMode = setup.DemoMode;
        ViewBag.UseQuickBooks = setup.UseQuickBooks;
        ViewBag.SavePicture = setup.SavePicture;
        return View();
    }

    // GET: Transaction/BasicTicket
    public IActionResult BasicTicket()
    {
        var setup = _setupCache.Get();
        ViewBag.NextTicket = setup.TicketNumber.ToString();
        ViewBag.CurrentWeight = _scaleService.GetCurrentWeight();
        PopulateDropdowns();
        return View();
    }

    // POST: Transaction/BasicTicket
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BasicTicket(Transaction transaction)
    {
        var setup = _db.AppSetup.First();
        while (_db.Transactions.Any(t => t.Ticket == setup.TicketNumber.ToString()))
        {
            setup.TicketNumber++;
        }
        transaction.Ticket = setup.TicketNumber.ToString();
        transaction.DateIn = transaction.DateIn == default ? DateTime.UtcNow : transaction.DateIn;
        transaction.DateOut = transaction.DateIn;
        transaction.OutWeight = transaction.InWeight;
        transaction.Void = false;

        setup.TicketNumber++;
        _db.AppSetup.Update(setup);

        _db.Transactions.Add(transaction);
        _db.SaveChanges();
        _setupCache.Invalidate();

        return RedirectToAction("CompletedTrucks", new { reset = "true" });
    }

    // POST: Transaction/Void/5
    [HttpPost("Transaction/Void/{id}")]
    public IActionResult ToggleVoid(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        transaction.Void = !transaction.Void;
        _db.SaveChanges();

        return Json(new { success = true, isVoid = transaction.Void });
    }

// POST: Transaction/DeleteInbound/5
    [HttpPost("Transaction/DeleteInbound/{id}")]
    public IActionResult DeleteInbound(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        _db.Transactions.Remove(transaction);
        _db.SaveChanges();

        return Json(new { success = true });
    }

    // GET: Transaction/Edit/5
    public IActionResult Edit(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        var setup = _setupCache.Get();
        ViewBag.SavePicture = setup.SavePicture;
        var ticketsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tickets");
        ViewBag.HasInImage = System.IO.File.Exists(Path.Combine(ticketsDir, $"{id}_in.jpg"));
        ViewBag.HasOutImage = System.IO.File.Exists(Path.Combine(ticketsDir, $"{id}_out.jpg"));

        PopulateDropdowns();
        return View(transaction);
    }

    // POST: Transaction/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, Transaction transaction)
    {
        if (id != transaction.Ticket) return BadRequest();

        var existing = _db.Transactions.Find(id);
        if (existing == null) return NotFound();

        existing.InWeight = transaction.InWeight;
        existing.OutWeight = transaction.OutWeight;
        existing.DateIn = transaction.DateIn;
        existing.DateOut = transaction.DateOut;
        existing.Customer = transaction.Customer;
        existing.Carrier = transaction.Carrier;
        existing.TruckId = transaction.TruckId;
        existing.Commodity = transaction.Commodity;
        existing.Location = transaction.Location;
        existing.Destination = transaction.Destination;
        existing.Notes = transaction.Notes;
        existing.Void = transaction.Void;
        existing.SentToQuickBooks = false; // Reset QB flag on edit

        _db.SaveChanges();

        return RedirectToAction("CompletedTrucks");
    }

    private static string TicketsImageDir => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tickets");

    private static bool HasImage(string ticket, string direction)
        => System.IO.File.Exists(Path.Combine(TicketsImageDir, $"{ticket}_{direction}.jpg"));

    /// <summary>
    /// Persist the truck's empty weight so future weigh-ins for the same truck can
    /// auto-recall it. Mirrors KioskController.UpdateRetainedTare — same matching
    /// rules and auto-create-on-miss behavior.
    /// </summary>
    private void UpdateRetainedTare(Transaction tx)
    {
        if (tx.OutWeight == null)
        {
            var msg = $"skipped for ticket {tx.Ticket}: OutWeight is null";
            _log.LogWarning(msg);
            Console.WriteLine($"[RetainedTare] {msg}");
            return;
        }
        var truckId = tx.TruckId?.Trim();
        var carrier = tx.Carrier?.Trim();
        if (string.IsNullOrEmpty(truckId) || string.IsNullOrEmpty(carrier))
        {
            var msg = $"skipped for ticket {tx.Ticket}: TruckId='{tx.TruckId}' Carrier='{tx.Carrier}' — both required";
            _log.LogWarning(msg);
            Console.WriteLine($"[RetainedTare] {msg}");
            return;
        }

        var tare = Math.Min(tx.InWeight, tx.OutWeight.Value);
        var when = tx.DateOut ?? DateTime.UtcNow;

        var truck = _db.Trucks.FirstOrDefault(t =>
            t.TruckId.ToLower() == truckId.ToLower() &&
            t.CarrierName.ToLower() == carrier.ToLower());

        if (truck == null)
        {
            truck = new Truck
            {
                TruckId = truckId,
                CarrierName = carrier,
                UseAtKiosk = true,
                Description = "Auto-created from weigh-out",
                RetainedTare = tare,
                RetainedTareUpdated = when
            };
            _db.Trucks.Add(truck);
            var msg = $"auto-created Truck '{truckId}' / '{carrier}' with tare {tare} lb (ticket {tx.Ticket})";
            _log.LogInformation(msg);
            Console.WriteLine($"[RetainedTare] {msg}");
        }
        else
        {
            truck.RetainedTare = tare;
            truck.RetainedTareUpdated = when;
            var msg = $"updated Truck '{truck.TruckId}' / '{truck.CarrierName}' to {tare} lb (ticket {tx.Ticket})";
            _log.LogInformation(msg);
            Console.WriteLine($"[RetainedTare] {msg}");
        }
    }

    // API: GET api/transactions/inbound
    [HttpGet("api/transactions/inbound")]
    public IActionResult GetInbound()
    {
        var transactions = _db.Transactions
            .Where(t => t.DateOut == null && !t.Void)
            .OrderByDescending(t => t.DateIn)
            .ToList()
            .Select(t => new
            {
                t.Ticket,
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.InWeight,
                DateIn = t.DateIn.AsUtc(),
                t.Location,
                t.Destination,
                t.Notes,
                t.ManualInbound,
                HasInImage = HasImage(t.Ticket, "in")
            })
            .ToList();

        return Json(transactions);
    }

    // API: GET api/transactions/completed
    [HttpGet("api/transactions/completed")]
    public IActionResult GetCompleted(DateTime? startDate, DateTime? endDate)
    {
        // Filter range: query strings come in as user-local dates in the
        // configured display TZ. Convert to UTC bounds so the comparison
        // against stored UTC values is correct regardless of host TZ.
        // endInclusive = midnight at the START of (endDate + 1) in the
        // display TZ — so an endDate of 2026-04-29 includes everything up
        // through 2026-04-29 23:59:59.999 local time.
        var localStart = startDate ?? DateTime.Today.AddDays(-30);
        var localEnd   = (endDate ?? DateTime.Today).Date.AddDays(1);
        var start = AppTimeZone.ToUtc(localStart);
        var endInclusive = AppTimeZone.ToUtc(localEnd);

        var transactions = _db.Transactions
            .Where(t => t.DateOut != null && t.DateIn >= start && t.DateIn < endInclusive)
            .OrderByDescending(t => t.DateIn)
            .ToList()
            .Select(t => new
            {
                t.Ticket,
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.InWeight,
                t.OutWeight,
                t.NetWeight,
                t.GrossWeight,
                t.TareWeight,
                DateIn = t.DateIn.AsUtc(),
                DateOut = t.DateOut.AsUtc(),
                t.Location,
                t.Destination,
                t.Notes,
                t.Void,
                t.SentToQuickBooks,
                HasInImage = HasImage(t.Ticket, "in"),
                HasOutImage = HasImage(t.Ticket, "out")
            })
            .ToList();

        return Json(transactions);
    }

    // API: POST api/transactions/update
    [HttpPost("api/transactions/update")]
    public IActionResult UpdateTransaction([FromBody] Transaction transaction)
    {
        var existing = _db.Transactions.Find(transaction.Ticket);
        if (existing == null) return NotFound();

        existing.InWeight = transaction.InWeight;
        existing.OutWeight = transaction.OutWeight;
        existing.DateIn = transaction.DateIn;
        existing.DateOut = transaction.DateOut;
        existing.Customer = transaction.Customer;
        existing.Carrier = transaction.Carrier;
        existing.TruckId = transaction.TruckId;
        existing.Commodity = transaction.Commodity;
        existing.Location = transaction.Location;
        existing.Destination = transaction.Destination;
        existing.Notes = transaction.Notes;
        existing.Void = transaction.Void;
        existing.SentToQuickBooks = false; // Reset QB flag on edit

        _db.SaveChanges();

        return Json(new { success = true });
    }

    // API: POST api/transactions/mark-sent-to-qb
    [HttpPost("api/transactions/mark-sent-to-qb")]
    public IActionResult MarkSentToQuickBooks([FromBody] List<string> ticketIds)
    {
        if (ticketIds == null || ticketIds.Count == 0)
            return BadRequest(new { error = "No ticket IDs provided." });

        var tickets = _db.Transactions
            .Where(t => ticketIds.Contains(t.Ticket))
            .ToList();

        foreach (var t in tickets)
            t.SentToQuickBooks = true;

        _db.SaveChanges();
        return Json(new { marked = tickets.Count });
    }

    // API: POST api/ticket/{id}/image?direction=in|out — upload ticket image
    [HttpPost("api/ticket/{id}/image")]
    public async Task<IActionResult> UploadTicketImage(string id, [FromQuery] string direction, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });
        if (direction != "in" && direction != "out")
            return BadRequest(new { error = "Direction must be 'in' or 'out'." });

        var dir = TicketsImageDir;
        Directory.CreateDirectory(dir);

        var filePath = Path.Combine(dir, $"{id}_{direction}.jpg");
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Notify all web clients that an image is available
        await _hub.Clients.All.SendAsync("ImageCaptured", new { ticket = id, direction });

        return Ok(new { success = true });
    }

    // API: GET api/ticket/{id}/image?direction=in|out — serve ticket image
    [HttpGet("api/ticket/{id}/image")]
    public IActionResult GetTicketImage(string id, [FromQuery] string direction)
    {
        if (direction != "in" && direction != "out")
            return BadRequest();

        var filePath = Path.Combine(TicketsImageDir, $"{id}_{direction}.jpg");
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        return PhysicalFile(filePath, "image/jpeg");
    }
}
