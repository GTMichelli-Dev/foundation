using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Reports;
using DevExpress.XtraReports.UI;

namespace BasicWeigh.Web.Controllers;

public class TicketController : Controller
{
    private readonly ScaleDbContext _db;
    private static readonly string ReportPath = Path.Combine(
        Directory.GetCurrentDirectory(), "Reports", "TicketReport.repx");

    public TicketController(ScaleDbContext db)
    {
        _db = db;
    }

    public IActionResult Print(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        var setup = _db.AppSetup.First();
        ViewBag.Header1 = setup.Header1;
        ViewBag.Header2 = setup.Header2;
        ViewBag.Header3 = setup.Header3;
        ViewBag.Header4 = setup.Header4;

        return View(transaction);
    }

    [HttpGet("api/ticket/{id}")]
    public IActionResult GetTicketData(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        var setup = _db.AppSetup.First();

        return Json(new
        {
            ticket = transaction.Ticket,
            dateIn = transaction.DateIn.ToString("MM/dd/yyyy hh:mm tt"),
            dateOut = transaction.DateOut?.ToString("MM/dd/yyyy hh:mm tt"),
            customer = transaction.Customer,
            carrier = transaction.Carrier,
            truckId = transaction.TruckId,
            commodity = transaction.Commodity,
            location = transaction.Location,
            destination = transaction.Destination,
            grossWeight = transaction.GrossWeight,
            tareWeight = transaction.TareWeight,
            netWeight = transaction.NetWeight,
            comment = transaction.Comment,
            notes = transaction.Notes,
            isVoid = transaction.Void,
            header1 = setup.Header1,
            header2 = setup.Header2,
            header3 = setup.Header3,
            header4 = setup.Header4
        });
    }

    // Report Designer - edit the ticket template
    public IActionResult Designer()
    {
        // Load existing .repx if saved, otherwise use default
        XtraReport report;
        if (System.IO.File.Exists(ReportPath))
        {
            report = XtraReport.FromFile(ReportPath);
        }
        else
        {
            report = new TicketReport();
        }
        return View(report);
    }

    // Save the report template from the designer
    [HttpPost]
    public IActionResult SaveReport()
    {
        // The DevExpress designer handles saving via its own API
        return Ok();
    }

    // Document Viewer - preview/print a specific ticket
    public IActionResult View(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        var setup = _db.AppSetup.First();

        // Load report template
        XtraReport report;
        if (System.IO.File.Exists(ReportPath))
        {
            report = XtraReport.FromFile(ReportPath);
        }
        else
        {
            report = new TicketReport();
        }

        // Set parameters
        SetParam(report, "Ticket", transaction.Ticket);
        SetParam(report, "DateIn", transaction.DateIn.ToString("MM/dd/yyyy hh:mm tt"));
        SetParam(report, "DateOut", transaction.DateOut?.ToString("MM/dd/yyyy hh:mm tt") ?? "");
        SetParam(report, "Customer", transaction.Customer ?? "");
        SetParam(report, "Carrier", transaction.Carrier ?? "");
        SetParam(report, "TruckId", transaction.TruckId ?? "");
        SetParam(report, "Commodity", transaction.Commodity ?? "");
        SetParam(report, "Location", transaction.Location ?? "");
        SetParam(report, "Destination", transaction.Destination ?? "");
        SetParam(report, "GrossWeight", transaction.GrossWeight.ToString("#,##0") + " lb");
        SetParam(report, "TareWeight", transaction.TareWeight.ToString("#,##0") + " lb");
        SetParam(report, "NetWeight", transaction.NetWeight.ToString("#,##0") + " lb");
        SetParam(report, "Comment", transaction.Comment ?? "");
        SetParam(report, "Notes", transaction.Notes ?? "");
        SetParam(report, "IsVoid", transaction.Void);
        SetParam(report, "Header1", setup.Header1 ?? "");
        SetParam(report, "Header2", setup.Header2 ?? "");
        SetParam(report, "Header3", setup.Header3 ?? "");
        SetParam(report, "Header4", setup.Header4 ?? "");

        ViewBag.TicketId = id;
        return View(report);
    }

    private void SetParam(XtraReport report, string name, object value)
    {
        var param = report.Parameters[name];
        if (param != null)
            param.Value = value;
    }
}
