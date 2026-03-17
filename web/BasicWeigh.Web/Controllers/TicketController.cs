using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Controllers;

public class TicketController : Controller
{
    private readonly ScaleDbContext _db;

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
}
