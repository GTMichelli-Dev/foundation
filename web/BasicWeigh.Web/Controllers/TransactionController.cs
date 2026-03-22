using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Services;

namespace BasicWeigh.Web.Controllers;

public class TransactionController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly IScaleService _scaleService;

    public TransactionController(ScaleDbContext db, IScaleService scaleService)
    {
        _db = db;
        _scaleService = scaleService;
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

        if (!string.IsNullOrEmpty(id))
        {
            var existing = _db.Transactions.Find(id);
            if (existing == null) return NotFound();
            if (existing.DateOut != null) return RedirectToAction("Edit", new { id });

            ViewBag.IsEdit = true;
            return View(existing);
        }

        var setup = _db.AppSetup.First();
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
    public IActionResult WeighIn(Transaction transaction, bool isEdit, bool goToWeighOut, bool manualWeight)
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
            transaction.Ticket = setup.TicketNumber.ToString();
            transaction.DateIn = transaction.DateIn == default ? DateTime.Now : transaction.DateIn;
            transaction.Void = false;
            transaction.ManualInbound = manualWeight;

            setup.TicketNumber++;
            _db.AppSetup.Update(setup);

            _db.Transactions.Add(transaction);
            _db.SaveChanges();
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
    public IActionResult WeighOut(string id, Transaction transaction, bool manualOutWeight, bool manualInWeight)
    {
        var existing = _db.Transactions.Find(id);
        if (existing == null) return NotFound();

        existing.InWeight = transaction.InWeight;
        existing.ManualInbound = manualInWeight;
        existing.OutWeight = transaction.OutWeight;
        existing.ManualOutbound = manualOutWeight;
        existing.DateOut = transaction.DateOut ?? DateTime.Now;
        existing.DateIn = transaction.DateIn;
        existing.Customer = transaction.Customer;
        existing.Carrier = transaction.Carrier;
        existing.TruckId = transaction.TruckId;
        existing.Commodity = transaction.Commodity;
        existing.Location = transaction.Location;
        existing.Destination = transaction.Destination;
        existing.Notes = transaction.Notes;

        _db.SaveChanges();

        return RedirectToAction("View", "Ticket", new { id });
    }

    // GET: Transaction/InboundTrucks
    public IActionResult InboundTrucks()
    {
        return View();
    }

    // GET: Transaction/CompletedTrucks
    public IActionResult CompletedTrucks()
    {
        return View();
    }

    // GET: Transaction/BasicTicket
    public IActionResult BasicTicket()
    {
        var setup = _db.AppSetup.First();
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
        transaction.Ticket = setup.TicketNumber.ToString();
        transaction.DateIn = transaction.DateIn == default ? DateTime.Now : transaction.DateIn;
        transaction.DateOut = transaction.DateIn;
        transaction.OutWeight = transaction.InWeight;
        transaction.Void = false;

        setup.TicketNumber++;
        _db.AppSetup.Update(setup);

        _db.Transactions.Add(transaction);
        _db.SaveChanges();

        return RedirectToAction("CompletedTrucks");
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

        _db.SaveChanges();

        return RedirectToAction("CompletedTrucks");
    }

    // API: GET api/transactions/inbound
    [HttpGet("api/transactions/inbound")]
    public IActionResult GetInbound()
    {
        var transactions = _db.Transactions
            .Where(t => t.DateOut == null && !t.Void)
            .OrderByDescending(t => t.DateIn)
            .Select(t => new
            {
                t.Ticket,
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.InWeight,
                t.DateIn,
                t.Location,
                t.Destination,
                t.Notes,
                t.ManualInbound
            })
            .ToList();

        return Json(transactions);
    }

    // API: GET api/transactions/completed
    [HttpGet("api/transactions/completed")]
    public IActionResult GetCompleted(DateTime? startDate, DateTime? endDate)
    {
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = endDate ?? DateTime.Today.AddDays(1);

        var transactions = _db.Transactions
            .Where(t => t.DateOut != null && t.DateIn >= start && t.DateIn < end)
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
                t.DateIn,
                t.DateOut,
                t.Location,
                t.Destination,
                t.Notes,
                t.Void
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

        _db.SaveChanges();

        return Json(new { success = true });
    }
}
