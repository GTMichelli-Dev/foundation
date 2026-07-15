using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Foundation.Web.Data;
using Foundation.Web.Hubs;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

public class ScaleController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly IHubContext<ScaleHub> _hub;
    private readonly AppSetupCache _setupCache;

    public ScaleController(ScaleDbContext db, IHubContext<ScaleHub> hub, AppSetupCache setupCache)
    {
        _db = db;
        _hub = hub;
        _setupCache = setupCache;
    }

    // MVC view for Scale Management — all data loads via SignalR
    public IActionResult Index()
    {
        var setup = _setupCache.Get();
        ViewBag.ScaleId = setup.ScaleId ?? "";
        return View();
    }

    // Legacy single-scale assignment (pre multi-scale). Kept so an old cached
    // page can't 404; writes the value nothing reads anymore.
    [HttpPost("api/scales/assignment")]
    public IActionResult SaveAssignment([FromBody] ScaleAssignmentDto dto)
    {
        var setup = _db.AppSetup.First();
        setup.ScaleId = dto.ScaleId;
        _db.SaveChanges();
        _setupCache.Invalidate();
        return Ok(new { success = true });
    }

    // ===== Named site scales (multi-scale) =====

    [HttpGet("api/scales")]
    public IActionResult GetScales() =>
        Json(_db.Scales.OrderBy(s => s.SortOrder).ThenBy(s => s.Name)
            .Select(s => new { s.Id, s.Name, s.HardwareId, s.SortOrder, s.Active, s.InboundPrinterId, s.OutboundPrinterId }).ToList());

    /// <summary>Active scales for the weigh-form / kiosk / simulator pickers.</summary>
    [HttpGet("api/scales/active")]
    public IActionResult GetActiveScales() =>
        Json(_db.Scales.Where(s => s.Active).OrderBy(s => s.SortOrder).ThenBy(s => s.Name)
            .Select(s => new { s.Id, s.Name, simulated = s.HardwareId == null || s.HardwareId == "" }).ToList());

    [HttpPost("api/scales")]
    public IActionResult AddScale([FromBody] Models.Scale scale)
    {
        var error = ValidateScale(scale, null);
        if (error != null) return BadRequest(new { error });

        scale.Id = 0;
        scale.Name = scale.Name.Trim();
        if (string.IsNullOrWhiteSpace(scale.HardwareId)) scale.HardwareId = null;
        _db.Scales.Add(scale);
        _db.SaveChanges();
        return Json(scale);
    }

    [HttpPut("api/scales/{id}")]
    public IActionResult UpdateScale(int id, [FromBody] Models.Scale scale)
    {
        var existing = _db.Scales.Find(id);
        if (existing == null) return NotFound();

        var error = ValidateScale(scale, id);
        if (error != null) return BadRequest(new { error });
        if (!scale.Active && !_db.Scales.Any(s => s.Id != id && s.Active))
            return BadRequest(new { error = "At least one scale must stay active." });

        existing.Name = scale.Name.Trim();
        existing.HardwareId = string.IsNullOrWhiteSpace(scale.HardwareId) ? null : scale.HardwareId;
        existing.SortOrder = scale.SortOrder;
        existing.Active = scale.Active;
        existing.InboundPrinterId = string.IsNullOrWhiteSpace(scale.InboundPrinterId) ? null : scale.InboundPrinterId;
        existing.OutboundPrinterId = string.IsNullOrWhiteSpace(scale.OutboundPrinterId) ? null : scale.OutboundPrinterId;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/scales/{id}")]
    public IActionResult DeleteScale(int id)
    {
        var existing = _db.Scales.Find(id);
        if (existing == null) return NotFound();
        if (_db.Scales.Count() <= 1)
            return BadRequest(new { error = "The last scale can't be deleted — the weigh forms need one. Deactivate or rename it instead." });

        _db.Scales.Remove(existing);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    private string? ValidateScale(Models.Scale scale, int? id)
    {
        if (string.IsNullOrWhiteSpace(scale.Name)) return "Scale name is required.";
        if (scale.Name.Trim().Length > 50) return "Scale name must be 50 characters or fewer.";
        if (_db.Scales.Any(s => s.Id != id && s.Name.ToLower() == scale.Name.Trim().ToLower()))
            return "A scale with that name already exists.";
        return null;
    }
}

public class ScaleAssignmentDto
{
    public string? ScaleId { get; set; }
}
