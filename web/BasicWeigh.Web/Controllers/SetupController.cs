using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Controllers;

public class SetupController : Controller
{
    private readonly ScaleDbContext _db;

    public SetupController(ScaleDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var setup = _db.AppSetup.First();
        return View(setup);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(AppSetup setup, IFormFile? iconFile, bool removeIcon = false)
    {
        var existing = _db.AppSetup.Find(setup.Id);
        if (existing == null) return NotFound();

        existing.Header1 = setup.Header1;
        existing.Header2 = setup.Header2;
        existing.Header3 = setup.Header3;
        existing.Header4 = setup.Header4;
        existing.TicketNumber = setup.TicketNumber;
        existing.TicketsPerPage = setup.TicketsPerPage;
        existing.DemoMode = setup.DemoMode;
        existing.KioskCount = setup.KioskCount;
        existing.Theme = setup.Theme;
        existing.PromptKioskCommodity = setup.PromptKioskCommodity;
        existing.PromptKioskCustomer = setup.PromptKioskCustomer;
        existing.PromptKioskCarrier = setup.PromptKioskCarrier;
        existing.PromptKioskTruckId = setup.PromptKioskTruckId;
        existing.PromptKioskLocation = setup.PromptKioskLocation;
        existing.PromptKioskDestinationOnInbound = setup.PromptKioskDestinationOnInbound;
        existing.PromptKioskDestinationOnOutbound = setup.PromptKioskDestinationOnOutbound;
        existing.KioskDarkMode = setup.KioskDarkMode;

        if (removeIcon)
        {
            existing.Icon = null;
            existing.IconContentType = null;
        }
        else if (iconFile is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            iconFile.CopyTo(ms);
            existing.Icon = ms.ToArray();
            existing.IconContentType = iconFile.ContentType;
        }

        _db.SaveChanges();

        TempData["Message"] = "Settings saved successfully.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetTicket(string reportName)
    {
        var reportsDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        var path = Path.Combine(reportsDir, reportName + ".repx");
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        var label = reportName == "KioskTicketReport" ? "Kiosk Inbound Ticket" : "Completed Ticket";
        TempData["Message"] = $"{label} has been reset to default format.";
        return RedirectToAction("Index");
    }

    [HttpGet("api/setup/icon")]
    public IActionResult GetIcon()
    {
        var setup = _db.AppSetup.First();
        if (setup.Icon != null && setup.IconContentType != null)
            return File(setup.Icon, setup.IconContentType);

        // Fall back to default SVG
        var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default-icon.svg");
        return PhysicalFile(defaultPath, "image/svg+xml");
    }
}
