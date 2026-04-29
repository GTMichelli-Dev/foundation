using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Services;

namespace BasicWeigh.Web.Controllers;

public class ReportController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly AppSetupCache _setupCache;

    public ReportController(ScaleDbContext db, AppSetupCache setupCache)
    {
        _db = db;
        _setupCache = setupCache;
    }

    public IActionResult Index()
    {
        var setup = _setupCache.Get();
        ViewBag.CompanyName = setup.Header1 ?? "Basic Weigh";
        ViewBag.SavePicture = setup.SavePicture;
        return View();
    }

    [HttpGet("api/reports/transactions")]
    public IActionResult GetTransactions(DateTime? startDate, DateTime? endDate)
    {
        // Filter: query strings come in as user-local dates. Convert to UTC
        // bounds so the comparison against stored UTC values is correct.
        var localStart = startDate ?? DateTime.Today.AddDays(-30);
        var localEnd   = (endDate ?? DateTime.Today).AddDays(1);
        var start = localStart.ToUniversalTime();
        var end   = localEnd.ToUniversalTime();

        // Count voided tickets in the date range
        var voidCount = _db.Transactions
            .Count(t => t.DateOut != null && t.Void && t.DateOut >= start && t.DateOut < end);

        // Only completed (has DateOut), non-voided trucks, filtered by DateOut
        var results = _db.Transactions
            .Where(t => t.DateOut != null && !t.Void && t.DateOut >= start && t.DateOut < end)
            .OrderByDescending(t => t.DateOut)
            .ToList()
            .Select(t => new
            {
                t.Ticket,
                DateIn = t.DateIn.AsUtc(),
                DateOut = t.DateOut.AsUtc(),
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.Location,
                t.Destination,
                t.InWeight,
                t.OutWeight,
                t.NetWeight,
                NetTons = Math.Round(t.NetWeight / 2000.0, 2),
                t.Notes,
                HasInImage = System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tickets", $"{t.Ticket}_in.jpg")),
                HasOutImage = System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tickets", $"{t.Ticket}_out.jpg"))
            })
            .ToList();

        return Json(new { data = results, voidCount });
    }

    [HttpGet("api/reports/voided")]
    public IActionResult GetVoided(DateTime? startDate, DateTime? endDate)
    {
        var localStart = startDate ?? DateTime.Today.AddDays(-30);
        var localEnd   = (endDate ?? DateTime.Today).AddDays(1);
        var start = localStart.ToUniversalTime();
        var end   = localEnd.ToUniversalTime();

        var results = _db.Transactions
            .Where(t => t.DateOut != null && t.Void && t.DateOut >= start && t.DateOut < end)
            .OrderByDescending(t => t.DateOut)
            .ToList()
            .Select(t => new
            {
                t.Ticket,
                DateIn = t.DateIn.AsUtc(),
                DateOut = t.DateOut.AsUtc(),
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.InWeight,
                t.OutWeight,
                t.NetWeight,
                t.Notes
            })
            .ToList();

        return Json(results);
    }
}
