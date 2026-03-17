using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Controllers;

public class ReportController : Controller
{
    private readonly ScaleDbContext _db;

    public ReportController(ScaleDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        ViewBag.Customers = _db.Customers.Where(c => c.Active).OrderBy(c => c.CustomerName).ToList();
        ViewBag.Carriers = _db.Carriers.Where(c => c.Active).OrderBy(c => c.CarrierName).ToList();
        ViewBag.Commodities = _db.Commodities.Where(c => c.Active).OrderBy(c => c.CommodityName).ToList();
        ViewBag.Locations = _db.Locations.Where(l => l.Active).OrderBy(l => l.LocationName).ToList();
        return View();
    }

    [HttpGet("api/reports/transactions")]
    public IActionResult GetTransactions(DateTime? startDate, DateTime? endDate, string? customer, string? commodity, string? location, string? carrier)
    {
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = endDate ?? DateTime.Today.AddDays(1);

        var query = _db.Transactions
            .Where(t => t.DateIn >= start && t.DateIn < end);

        if (!string.IsNullOrEmpty(customer))
            query = query.Where(t => t.Customer == customer);
        if (!string.IsNullOrEmpty(commodity))
            query = query.Where(t => t.Commodity == commodity);
        if (!string.IsNullOrEmpty(location))
            query = query.Where(t => t.Location == location);
        if (!string.IsNullOrEmpty(carrier))
            query = query.Where(t => t.Carrier == carrier);

        var results = query
            .OrderByDescending(t => t.DateIn)
            .ToList()
            .Select(t => new
            {
                t.Ticket,
                t.DateIn,
                t.DateOut,
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.Location,
                t.Destination,
                t.InWeight,
                t.OutWeight,
                t.NetWeight,
                t.Void
            })
            .ToList();

        return Json(results);
    }
}
