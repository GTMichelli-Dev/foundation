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
        var setup = _db.AppSetup.First();
        ViewBag.CompanyName = setup.Header1 ?? "Basic Weigh";
        return View();
    }

    [HttpGet("api/reports/transactions")]
    public IActionResult GetTransactions(DateTime? startDate, DateTime? endDate)
    {
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = (endDate ?? DateTime.Today).AddDays(1);

        // Only completed (has DateOut), non-voided trucks, filtered by DateOut
        var results = _db.Transactions
            .Where(t => t.DateOut != null && !t.Void && t.DateOut >= start && t.DateOut < end)
            .OrderByDescending(t => t.DateOut)
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
                NetTons = Math.Round(t.NetWeight / 2000.0, 2),
                t.Notes
            })
            .ToList();

        return Json(results);
    }
}
