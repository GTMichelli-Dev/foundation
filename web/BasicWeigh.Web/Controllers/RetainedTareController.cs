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

    /// <summary>
    /// Look up the retained tare for a (carrier, truckId) pair. Returns 404 if no
    /// truck found, 200 with retainedTare=null if the truck exists but has no
    /// stored tare. The caller can use this to decide whether to show the
    /// "TARE RECALLED" banner on the WeighIn page.
    /// </summary>
    [HttpGet("api/retainedtares/lookup")]
    public IActionResult Lookup([FromQuery] string carrier, [FromQuery] string truckId)
    {
        if (string.IsNullOrWhiteSpace(carrier) || string.IsNullOrWhiteSpace(truckId))
            return BadRequest(new { message = "carrier and truckId are required" });

        var c = carrier.Trim().ToLower();
        var t = truckId.Trim().ToLower();
        var truck = _db.Trucks.FirstOrDefault(x =>
            x.TruckId.ToLower() == t && x.CarrierName.ToLower() == c);

        if (truck == null) return NotFound();

        return Ok(new
        {
            truckId = truck.TruckId,
            carrier = truck.CarrierName,
            retainedTare = truck.RetainedTare,
            retainedTareUpdated = truck.RetainedTareUpdated
        });
    }

    public class UpdateRequest
    {
        public int? RetainedTare { get; set; }
    }
}
