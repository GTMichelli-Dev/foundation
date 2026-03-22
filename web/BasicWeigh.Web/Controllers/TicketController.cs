using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Hubs;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Reports;
using BasicWeigh.Web.Services;
using DevExpress.XtraReports.UI;
using DevExpress.Drawing;

namespace BasicWeigh.Web.Controllers;

public class TicketController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly PrintQueueService _printQueue;
    private readonly IHubContext<ScaleHub> _hub;
    private static readonly string ReportPath = Path.Combine(
        Directory.GetCurrentDirectory(), "Reports", "TicketReport.repx");

    public TicketController(ScaleDbContext db, PrintQueueService printQueue, IHubContext<ScaleHub> hub)
    {
        _db = db;
        _printQueue = printQueue;
        _hub = hub;
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
            notes = transaction.Notes,
            isVoid = transaction.Void,
            header1 = setup.Header1,
            header2 = setup.Header2,
            header3 = setup.Header3,
            header4 = setup.Header4
        });
    }

    // Report Designer - edit a ticket template
    public IActionResult Designer(string reportName = "TicketReport")
    {
        return View("Designer", reportName);
    }

    // Document Viewer - preview/print a specific ticket
    public new IActionResult View(string id)
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

        // Set logo image on the picture box
        SetLogoImage(report, setup);

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
        SetParam(report, "Notes", transaction.Notes ?? "");
        SetParam(report, "IsVoid", transaction.Void);
        SetParam(report, "Header1", setup.Header1 ?? "");
        SetParam(report, "Header2", setup.Header2 ?? "");
        SetParam(report, "Header3", setup.Header3 ?? "");
        SetParam(report, "Header4", setup.Header4 ?? "");

        ViewBag.TicketId = id;
        return View(report);
    }

    // Kiosk Ticket - print inbound kiosk ticket with barcode
    public IActionResult KioskView(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        var setup = _db.AppSetup.First();

        var kioskPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "KioskTicketReport.repx");
        XtraReport report;
        if (System.IO.File.Exists(kioskPath))
        {
            report = XtraReport.FromFile(kioskPath);
        }
        else
        {
            report = new KioskTicketReport();
        }

        SetParam(report, "Ticket", transaction.Ticket);
        SetParam(report, "DateIn", transaction.DateIn.ToString("MM/dd/yyyy hh:mm tt"));
        SetParam(report, "Customer", transaction.Customer ?? "");
        SetParam(report, "Carrier", transaction.Carrier ?? "");
        SetParam(report, "TruckId", transaction.TruckId ?? "");
        SetParam(report, "Commodity", transaction.Commodity ?? "");
        SetParam(report, "Location", transaction.Location ?? "");
        SetParam(report, "InWeight", transaction.InWeight.ToString("#,##0") + " lb");
        SetParam(report, "Header1", setup.Header1 ?? "");
        SetParam(report, "Header2", setup.Header2 ?? "");
        SetParam(report, "Header3", setup.Header3 ?? "");
        SetParam(report, "Header4", setup.Header4 ?? "");

        ViewBag.TicketId = id;
        return View(report);
    }

    [HttpGet("api/ticket/{id}/pdf")]
    public IActionResult GetTicketPdf(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        var setup = _db.AppSetup.First();

        // Use KioskTicketReport for inbound (not yet completed), TicketReport for completed
        bool isInbound = transaction.DateOut == null;

        XtraReport report;
        if (isInbound)
        {
            var kioskPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "KioskTicketReport.repx");
            report = System.IO.File.Exists(kioskPath)
                ? XtraReport.FromFile(kioskPath)
                : new KioskTicketReport();

            SetParam(report, "Ticket", transaction.Ticket);
            SetParam(report, "DateIn", transaction.DateIn.ToString("MM/dd/yyyy hh:mm tt"));
            SetParam(report, "Customer", transaction.Customer ?? "");
            SetParam(report, "Carrier", transaction.Carrier ?? "");
            SetParam(report, "TruckId", transaction.TruckId ?? "");
            SetParam(report, "Commodity", transaction.Commodity ?? "");
            SetParam(report, "Location", transaction.Location ?? "");
            SetParam(report, "InWeight", transaction.InWeight.ToString("#,##0") + " lb");
            SetParam(report, "Header1", setup.Header1 ?? "");
            SetParam(report, "Header2", setup.Header2 ?? "");
            SetParam(report, "Header3", setup.Header3 ?? "");
            SetParam(report, "Header4", setup.Header4 ?? "");
        }
        else
        {
            report = System.IO.File.Exists(ReportPath)
                ? XtraReport.FromFile(ReportPath)
                : new TicketReport();

            SetLogoImage(report, setup);
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
            SetParam(report, "Notes", transaction.Notes ?? "");
            SetParam(report, "IsVoid", transaction.Void);
            SetParam(report, "Header1", setup.Header1 ?? "");
            SetParam(report, "Header2", setup.Header2 ?? "");
            SetParam(report, "Header3", setup.Header3 ?? "");
            SetParam(report, "Header4", setup.Header4 ?? "");
        }

        using var ms = new MemoryStream();
        report.ExportToPdf(ms);
        ms.Position = 0;
        return File(ms.ToArray(), "application/pdf", $"ticket-{id}.pdf");
    }

    /// <summary>
    /// Reprint a ticket via Scale or Remote Printer.
    /// </summary>
    [HttpPost("api/ticket/{id}/reprint")]
    public async Task<IActionResult> Reprint(string id)
    {
        var transaction = _db.Transactions.Find(id);
        if (transaction == null) return NotFound();

        var setup = _db.AppSetup.First();

        if (setup.RemotePrintMode == "Scale")
        {
            _printQueue.Enqueue(id);
        }
        else if (setup.RemotePrintMode == "RemotePrinter")
        {
            _printQueue.Enqueue(id);
            await _hub.Clients.Group("PrintClients").SendAsync("PrintTicket",
                new { ticketId = id, pdfUrl = $"/api/ticket/{id}/pdf" });
        }
        else
        {
            return BadRequest(new { success = false, message = "Remote printing is not enabled." });
        }

        return Json(new { success = true, mode = setup.RemotePrintMode });
    }

    /// <summary>
    /// Called by the scale or remote printer to confirm a print job was received.
    /// Broadcasts PrintConfirmed to all browser clients.
    /// </summary>
    [HttpPost("api/ticket/{id}/printed")]
    public async Task<IActionResult> PrintConfirmed(string id)
    {
        var setup = _db.AppSetup.First();
        var label = setup.RemotePrintMode == "Scale" ? "Scale" : "Remote Printer";
        await _hub.Clients.All.SendAsync("PrintConfirmed", new { ticketId = id, printer = label });
        return Json(new { success = true });
    }

    private void SetLogoImage(XtraReport report, AppSetup setup)
    {
        var picBox = report.FindControl("picLogo", true) as XRPictureBox;
        if (picBox == null) return;

        byte[]? iconBytes = setup.Icon;
        if (iconBytes == null)
        {
            // Load default icon
            var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default-icon.svg");
            if (System.IO.File.Exists(defaultPath))
                iconBytes = System.IO.File.ReadAllBytes(defaultPath);
        }

        if (iconBytes != null)
        {
            try
            {
                picBox.ImageSource = new DevExpress.XtraPrinting.Drawing.ImageSource(iconBytes);
            }
            catch
            {
                // SVG or unsupported format - skip logo
            }
        }
    }

    private void SetParam(XtraReport report, string name, object value)
    {
        var param = report.Parameters[name];
        if (param != null)
            param.Value = value;
    }
}
