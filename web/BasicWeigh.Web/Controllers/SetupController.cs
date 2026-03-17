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
    public IActionResult Index(AppSetup setup)
    {
        var existing = _db.AppSetup.Find(setup.Id);
        if (existing == null) return NotFound();

        existing.Header1 = setup.Header1;
        existing.Header2 = setup.Header2;
        existing.Header3 = setup.Header3;
        existing.Header4 = setup.Header4;
        existing.TicketNumber = setup.TicketNumber;
        existing.TicketsPerPage = setup.TicketsPerPage;

        _db.SaveChanges();

        TempData["Message"] = "Settings saved successfully.";
        return RedirectToAction("Index");
    }
}
