using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Controllers;

public class RetainedTareController : Controller
{
    private readonly ScaleDbContext _db;

    public RetainedTareController(ScaleDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var trucks = _db.Trucks
            .OrderByDescending(t => t.RetainedTare != null)
            .ThenBy(t => t.CarrierName)
            .ThenBy(t => t.TruckId)
            .ToList();
        return View(trucks);
    }

    [HttpPost("api/retainedtares/{id:int}/clear")]
    public IActionResult Clear(int id)
    {
        var truck = _db.Trucks.Find(id);
        if (truck == null) return NotFound();
        truck.RetainedTare = null;
        truck.RetainedTareUpdated = null;
        _db.SaveChanges();
        return Ok(new { id, truckId = truck.TruckId, carrier = truck.CarrierName });
    }

    [HttpPost("api/retainedtares/{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateRequest body)
    {
        var truck = _db.Trucks.Find(id);
        if (truck == null) return NotFound();
        if (body.RetainedTare.HasValue && body.RetainedTare.Value < 0)
            return BadRequest(new { message = "Tare must be zero or positive." });

        truck.RetainedTare = body.RetainedTare;
        truck.RetainedTareUpdated = body.RetainedTare.HasValue ? DateTime.Now : (DateTime?)null;
        _db.SaveChanges();
        return Ok(new
        {
            id,
            truckId = truck.TruckId,
            carrier = truck.CarrierName,
            retainedTare = truck.RetainedTare,
            retainedTareUpdated = truck.RetainedTareUpdated
        });
    }

    [HttpPost("api/retainedtares/clear-all")]
    public IActionResult ClearAll()
    {
        var count = 0;
        foreach (var truck in _db.Trucks.Where(t => t.RetainedTare != null))
        {
            truck.RetainedTare = null;
            truck.RetainedTareUpdated = null;
            count++;
        }
        _db.SaveChanges();
        return Ok(new { cleared = count });
    }

    public class UpdateRequest
    {
        public int? RetainedTare { get; set; }
    }
}
