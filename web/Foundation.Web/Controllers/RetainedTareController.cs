using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Foundation.Web.Data;
using Foundation.Web.Models;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

public class RetainedTareController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly AppSetupCache _setupCache;

    public RetainedTareController(ScaleDbContext db, AppSetupCache setupCache)
    {
        _db = db;
        _setupCache = setupCache;
    }

    public IActionResult Index()
    {
        SweepStaleTares();
        var trucks = _db.Trucks
            .OrderByDescending(t => t.RetainedTare != null)
            .ThenBy(t => t.CarrierName)
            .ThenBy(t => t.TruckId)
            .ToList();
        return View(trucks);
    }

    /// <summary>
    /// Auto-expire any retained tare whose update date is before today. Called on
    /// the admin page and the lookup endpoint so the displayed/used data is always
    /// current. Called outside a SaveChanges block — the caller persists.
    /// </summary>
    private void SweepStaleTares()
    {
        if (!_setupCache.Get().AutoClearStaleRetainedTare) return;
        var today = DateTime.Today;
        var stale = _db.Trucks
            .Where(t => t.RetainedTare != null
                && (t.RetainedTareUpdated == null || t.RetainedTareUpdated < today))
            .ToList();
        if (stale.Count == 0) return;
        foreach (var t in stale)
        {
            t.RetainedTare = null;
            t.RetainedTareUpdated = null;
        }
        _db.SaveChanges();
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

        // Tares from a previous date are auto-expired before reporting (gated
        // on AutoClearStaleRetainedTare so the operator can disable midnight
        // expiry for fleets where tares are stable across days).
        if (_setupCache.Get().AutoClearStaleRetainedTare
            && truck.RetainedTare.HasValue
            && (truck.RetainedTareUpdated?.Date ?? DateTime.MinValue) < DateTime.Today)
        {
            truck.RetainedTare = null;
            truck.RetainedTareUpdated = null;
            _db.SaveChanges();
        }

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
