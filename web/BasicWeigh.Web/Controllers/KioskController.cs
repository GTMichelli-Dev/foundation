using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Hubs;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Services;

namespace BasicWeigh.Web.Controllers;

public class KioskController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly IScaleService _scaleService;
    private readonly IHubContext<ScaleHub> _hub;
    private readonly AppSetupCache _setupCache;
    private readonly ILogger<KioskController> _log;

    public KioskController(ScaleDbContext db, IScaleService scaleService, IHubContext<ScaleHub> hub, AppSetupCache setupCache, ILogger<KioskController> log)
    {
        _db = db;
        _scaleService = scaleService;
        _hub = hub;
        _setupCache = setupCache;
        _log = log;
    }

    public IActionResult Index([FromQuery(Name = "service-id")] string? serviceId = null,
                               [FromQuery(Name = "printer-id")] string? printerId = null)
    {
        var setup = _setupCache.Get();
        ViewBag.ServiceId = serviceId ?? "";
        ViewBag.PrinterId = printerId ?? "";
        ViewBag.HasPrinter = !string.IsNullOrEmpty(serviceId) && !string.IsNullOrEmpty(printerId);
        ViewBag.ScaleId = setup.ScaleId ?? "";
        return View(setup);
    }

    [HttpGet("api/kiosk/lists")]
    public IActionResult GetLists()
    {
        var commodities = _db.Commodities
            .Where(c => c.Active && c.UseAtKiosk)
            .OrderBy(c => c.CommodityName)
            .Select(c => c.CommodityName)
            .ToList();

        var customers = _db.Customers
            .Where(c => c.Active && c.UseAtKiosk)
            .OrderBy(c => c.CustomerName)
            .Select(c => c.CustomerName)
            .ToList();

        // Carriers = dedicated carriers + all active kiosk customers
        var carrierNames = _db.Carriers
            .Where(c => c.Active && c.UseAtKiosk)
            .Select(c => c.CarrierName)
            .ToList();
        var customerNames = _db.Customers
            .Where(c => c.Active && c.UseAtKiosk)
            .Select(c => c.CustomerName)
            .ToList();
        var carriers = carrierNames.Union(customerNames).OrderBy(n => n).ToList();

        var locations = _db.Locations
            .Where(l => l.Active && l.UseAtKiosk)
            .OrderBy(l => l.LocationName)
            .Select(l => l.LocationName)
            .ToList();

        var destinations = _db.Destinations
            .Where(d => d.Active && d.UseAtKiosk)
            .OrderBy(d => d.DestinationName)
            .Select(d => d.DestinationName)
            .ToList();

        return Json(new { commodities, customers, carriers, locations, destinations });
    }

    [HttpGet("api/kiosk/trucks/{carrier}")]
    public IActionResult GetTrucks(string carrier)
    {
        var trucks = _db.Trucks
            .Where(t => t.CarrierName == carrier && t.UseAtKiosk)
            .OrderBy(t => t.TruckId)
            .Select(t => t.TruckId)
            .ToList();

        return Json(trucks);
    }

    /// <summary>
    /// Find the open inbound ticket (no DateOut, not voided) for a given
    /// (Carrier, TruckId). Used by the kiosk so a driver who walks through
    /// the inbound prompt sequence on a return trip is automatically
    /// switched to the weigh-out flow instead of being forced to back out
    /// and key in a ticket number.
    /// </summary>
    [HttpGet("api/kiosk/open-ticket-for-truck")]
    public IActionResult FindOpenTicketForTruck([FromQuery] string carrier, [FromQuery] string truckId)
    {
        if (string.IsNullOrWhiteSpace(carrier) || string.IsNullOrWhiteSpace(truckId))
            return BadRequest(new { message = "carrier and truckId are required" });

        var c = carrier.Trim();
        var t = truckId.Trim();
        var transaction = _db.Transactions
            .Where(x => !x.Void && x.DateOut == null && x.Carrier == c && x.TruckId == t)
            .OrderByDescending(x => x.DateIn)
            .FirstOrDefault();

        if (transaction == null) return NotFound(new { message = "No open ticket" });

        return Json(new
        {
            ticket = transaction.Ticket,
            inWeight = transaction.InWeight,
            dateIn = transaction.DateIn.AsUtc(),
            customer = transaction.Customer,
            carrier = transaction.Carrier,
            truckId = transaction.TruckId,
            commodity = transaction.Commodity,
            location = transaction.Location,
            destination = transaction.Destination
        });
    }

    [HttpGet("api/kiosk/ticket/{ticketNumber}")]
    public IActionResult FindTicket(string ticketNumber)
    {
        var transaction = _db.Transactions
            .FirstOrDefault(t => t.Ticket == ticketNumber);

        if (transaction == null)
            return NotFound(new { message = "Ticket not found" });

        if (transaction.Void)
            return BadRequest(new { message = "Ticket is voided" });

        if (transaction.DateOut != null)
            return BadRequest(new { message = "Ticket already completed" });

        return Json(new
        {
            ticket = transaction.Ticket,
            inWeight = transaction.InWeight,
            dateIn = transaction.DateIn,
            customer = transaction.Customer,
            carrier = transaction.Carrier,
            truckId = transaction.TruckId,
            commodity = transaction.Commodity,
            location = transaction.Location,
            destination = transaction.Destination
        });
    }

    [HttpPost("api/kiosk/weighin")]
    public async Task<IActionResult> WeighIn([FromBody] KioskWeighInRequest request)
    {
        var setup = _db.AppSetup.First();
        // Ensure ticket number doesn't collide with existing tickets
        while (_db.Transactions.Any(t => t.Ticket == setup.TicketNumber.ToString()))
        {
            setup.TicketNumber++;
        }
        var ticketNumber = setup.TicketNumber.ToString();

        // Look up retained tare for this truck. Match the (TruckId, Carrier) pair
        // that already uniquely identifies a truck (ScaleDbContext unique index).
        // Skip entirely if the feature toggle is off.
        Truck? truck = null;
        if (setup.UseRetainedTare
            && !string.IsNullOrEmpty(request.TruckId)
            && !string.IsNullOrEmpty(request.Carrier))
        {
            truck = _db.Trucks.FirstOrDefault(t =>
                t.TruckId == request.TruckId && t.CarrierName == request.Carrier);

            // Tares from a previous date are auto-expired — load may have changed
            // overnight, so a stale tare can't be trusted. Gated on
            // AutoClearStaleRetainedTare so an operator can disable midnight
            // expiry if their fleet's tares are stable across days.
            if (setup.AutoClearStaleRetainedTare
                && truck?.RetainedTare.HasValue == true
                && (truck.RetainedTareUpdated?.Date ?? DateTime.MinValue) < DateTime.Today)
            {
                Console.WriteLine($"[RetainedTare] cleared stale tare for '{truck.TruckId}' / '{truck.CarrierName}' (last seen {truck.RetainedTareUpdated:yyyy-MM-dd})");
                _log.LogInformation("RetainedTare: cleared stale tare for {TruckId}/{Carrier} (last seen {When})",
                    truck.TruckId, truck.CarrierName, truck.RetainedTareUpdated);
                truck.RetainedTare = null;
                truck.RetainedTareUpdated = null;
            }
        }
        bool tareApplied = truck?.RetainedTare.HasValue == true;
        var now = DateTime.UtcNow;

        var transaction = new Transaction
        {
            Ticket = ticketNumber,
            InWeight = request.Weight,
            DateIn = now,
            Commodity = request.Commodity,
            Customer = request.Customer,
            Carrier = request.Carrier,
            TruckId = request.TruckId,
            Location = request.Location,
            Destination = request.Destination,
            Void = false,
            ManualInbound = false
        };

        if (tareApplied)
        {
            transaction.OutWeight = truck!.RetainedTare;
            transaction.DateOut = now;
            transaction.ManualOutbound = false;
        }

        setup.TicketNumber++;
        _db.AppSetup.Update(setup);
        _db.Transactions.Add(transaction);
        _db.SaveChanges();
        _setupCache.Invalidate();

        // Notify all clients
        if (tareApplied)
        {
            await _hub.Clients.All.SendAsync("TicketCompleted",
                new { ticket = ticketNumber, type = "weighout" });
        }
        else
        {
            await _hub.Clients.All.SendAsync("TicketCreated",
                new { ticket = ticketNumber, type = "weighin" });
        }

        // Camera capture: a tare-completed ticket is a weigh-out (use outbound camera);
        // a regular weigh-in uses the inbound camera. Same convention as the admin flow.
        if (setup.SavePicture)
        {
            await SendCameraCapture(
                ticketNumber,
                tareApplied ? "out" : "in",
                tareApplied ? setup.OutboundCameraId : setup.InboundCameraId);
        }

        // Print: a tare-completed ticket is effectively a weigh-out, route to the outbound printer.
        await SendPrintCommand(ticketNumber, tareApplied ? "weighout" : "weighin", request.PrinterId);

        return Json(new
        {
            ticket = ticketNumber,
            inWeight = transaction.InWeight,
            outWeight = transaction.OutWeight,
            dateOut = transaction.DateOut,
            tareApplied,
            retainedTare = truck?.RetainedTare,
            retainedTareUpdated = truck?.RetainedTareUpdated
        });
    }

    [HttpPost("api/kiosk/weighout")]
    public async Task<IActionResult> WeighOut([FromBody] KioskWeighOutRequest request)
    {
        var transaction = _db.Transactions
            .FirstOrDefault(t => t.Ticket == request.Ticket && !t.Void && t.DateOut == null);

        if (transaction == null)
            return NotFound(new { message = "Ticket not found" });

        transaction.OutWeight = request.Weight;
        transaction.DateOut = DateTime.UtcNow;
        transaction.ManualOutbound = false;

        // Outbound-only prompts can override the values captured at weigh-in.
        // Empty / null means "no change" — the kiosk JS already coerces a blank
        // selection to null so it doesn't blow away an existing value here.
        if (!string.IsNullOrEmpty(request.Destination)) transaction.Destination = request.Destination;
        if (!string.IsNullOrEmpty(request.Commodity))   transaction.Commodity   = request.Commodity;
        if (!string.IsNullOrEmpty(request.Customer))    transaction.Customer    = request.Customer;
        if (!string.IsNullOrEmpty(request.Location))    transaction.Location    = request.Location;

        // Persist retained tare on the matching truck (feature-gated). Tare = lower of
        // the two weights, matching how Transaction.TareWeight is computed.
        var useRetainedTare = _setupCache.Get().UseRetainedTare;
        var weighOutMsg = $"WeighOut ticket {transaction.Ticket}: UseRetainedTare={useRetainedTare} TruckId='{transaction.TruckId}' Carrier='{transaction.Carrier}'";
        _log.LogInformation(weighOutMsg);
        Console.WriteLine($"[RetainedTare] {weighOutMsg}");
        if (useRetainedTare)
        {
            UpdateRetainedTare(transaction);
        }

        _db.SaveChanges();

        // Notify all clients that a ticket was completed
        await _hub.Clients.All.SendAsync("TicketCompleted", new { ticket = transaction.Ticket, type = "weighout" });

        // Camera capture (outbound) — same convention as the admin web flow.
        var outSetup = _setupCache.Get();
        if (outSetup.SavePicture)
        {
            await SendCameraCapture(transaction.Ticket, "out", outSetup.OutboundCameraId);
        }

        // Print the ticket
        await SendPrintCommand(transaction.Ticket.ToString(), "weighout", request.PrinterId);

        return Json(new { ticket = transaction.Ticket });
    }

    /// <summary>
    /// Send a CaptureImage command to the camera service identified by the
    /// "serviceId:cameraId" string from AppSetup. No-op if the setting is empty.
    /// </summary>
    private async Task SendCameraCapture(string ticketId, string direction, string? cameraIdSetting)
    {
        if (string.IsNullOrEmpty(cameraIdSetting)) return;
        var parts = cameraIdSetting.Split(':', 2);
        var serviceId = parts.Length > 1 ? parts[0] : "default";
        var cameraId = parts.Length > 1 ? parts[1] : parts[0];
        await _hub.Clients.Group($"Camera_{serviceId}").SendAsync("CaptureImage",
            new { ticket = ticketId, direction, cameraId });
    }

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

        // Match the kiosk's existing (TruckId, CarrierName) unique key. Use a
        // case-insensitive comparison so subtle casing/spacing drift between the
        // master row and the value typed at the kiosk doesn't silently skip the
        // update.
        var truck = _db.Trucks.FirstOrDefault(t =>
            t.TruckId.ToLower() == truckId.ToLower() &&
            t.CarrierName.ToLower() == carrier.ToLower());

        if (truck == null)
        {
            // Master data doesn't have this truck yet (e.g. it was deleted, or the
            // kiosk wrote a value not in the dropdown). Create it so the retained-
            // tare feature works without forcing the operator to set up master data
            // first. The admin page will show it and the operator can edit/clear.
            truck = new Truck
            {
                TruckId = truckId,
                CarrierName = carrier,
                UseAtKiosk = true,
                Description = "Auto-created from kiosk weigh-out",
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

    [HttpPost("api/kiosk/reprint/{ticketId}")]
    public async Task<IActionResult> Reprint(string ticketId, [FromQuery] string? printerId = null)
    {
        var transaction = _db.Transactions.Find(ticketId);
        if (transaction == null)
            return NotFound(new { message = "Ticket not found" });

        var type = transaction.DateOut != null ? "weighout" : "weighin";

        await SendPrintCommand(ticketId, type, printerId);

        return Ok(new { message = "Reprint requested" });
    }

    /// <summary>
    /// Sends a print command to the correct print service.
    /// printerId format: "serviceId:printerId" (e.g., "office-1:BIXOLON BK3-3")
    /// If not set and demo mode: uses "KioskPrinter"
    /// If not set and not demo: returns without printing (kiosk should prompt)
    /// </summary>
    private async Task SendPrintCommand(string ticketId, string type, string? printerId)
    {
        var setup = _setupCache.Get();

        // If no printer specified, use defaults
        if (string.IsNullOrEmpty(printerId))
        {
            if (setup.DemoMode)
            {
                // In demo mode, use a virtual "KioskPrinter" so the flow works
                printerId = "demo:KioskPrinter";
            }
            else
            {
                // Use the inbound/outbound printer assignment from setup
                printerId = type == "weighout"
                    ? setup.OutboundPrinterId
                    : setup.InboundPrinterId;
            }
        }

        if (string.IsNullOrEmpty(printerId)) return;

        // Browser printing — handled client-side, skip server-side print command
        if (printerId.Equals("Browser:Browser", StringComparison.OrdinalIgnoreCase)) return;

        // Split serviceId:printerId
        var parts = printerId.Split(':', 2);
        var serviceId = parts.Length > 1 ? parts[0] : "";
        var printerName = parts.Length > 1 ? parts[1] : parts[0];

        if (!string.IsNullOrEmpty(serviceId))
        {
            // Route to specific service
            await _hub.Clients.Group($"Print_{serviceId}").SendAsync("PrintTicket",
                new { ticketId, type, printerId = printerName });
        }
        else
        {
            // Broadcast to all print services
            await _hub.Clients.Group("PrintClients").SendAsync("PrintTicket",
                new { ticketId, type, printerId = printerName });
        }
    }

    public class KioskWeighInRequest
    {
        public int Weight { get; set; }
        public string? Commodity { get; set; }
        public string? Customer { get; set; }
        public string? Carrier { get; set; }
        public string? TruckId { get; set; }
        public string? Location { get; set; }
        public string? Destination { get; set; }
        /// <summary>
        /// Optional printer in "serviceId:printerId" format.
        /// If not set: demo mode uses "demo:KioskPrinter", normal mode uses inbound printer from Setup.
        /// </summary>
        public string? PrinterId { get; set; }
    }

    public class KioskWeighOutRequest
    {
        public string Ticket { get; set; } = string.Empty;
        public int Weight { get; set; }
        public string? Destination { get; set; }
        // Outbound-only prompt values. Each is optional — empty means the
        // operator wasn't prompted, or skipped, and the existing transaction
        // value is preserved.
        public string? Commodity { get; set; }
        public string? Customer { get; set; }
        public string? Location { get; set; }
        /// <summary>
        /// Optional printer in "serviceId:printerId" format.
        /// If not set: demo mode uses "demo:KioskPrinter", normal mode uses outbound printer from Setup.
        /// </summary>
        public string? PrinterId { get; set; }
    }
}
