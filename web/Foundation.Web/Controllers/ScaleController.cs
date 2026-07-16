using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Foundation.Web.Data;
using Foundation.Web.Hubs;
using Foundation.Web.Models;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

public class ScaleController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly IHubContext<ScaleHub> _hub;
    private readonly AppSetupCache _setupCache;
    private readonly IConfiguration _config;

    public ScaleController(ScaleDbContext db, IHubContext<ScaleHub> hub, AppSetupCache setupCache, IConfiguration config)
    {
        _db = db;
        _hub = hub;
        _setupCache = setupCache;
        _config = config;
    }

    /// <summary>Site scale cap, configurable via appsettings "MaxScales" (default 4).</summary>
    private int MaxScales => Math.Max(1, _config.GetValue<int>("MaxScales", 4));

    // MVC view for Scale Management — all data loads via SignalR
    public IActionResult Index()
    {
        var setup = _setupCache.Get();
        ViewBag.ScaleId = setup.ScaleId ?? "";
        ViewBag.MaxScales = MaxScales;
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
            .Select(s => new { s.Id, s.Name, s.HardwareId, s.SiteId, s.SortOrder, s.Active, s.InboundPrinterId, s.OutboundPrinterId }).ToList());

    /// <summary>Active scales for the weigh-form / kiosk / simulator pickers,
    /// limited to the operator's current location (navbar picker).</summary>
    [HttpGet("api/scales/active")]
    public IActionResult GetActiveScales()
    {
        var siteId = SiteContext.CurrentSiteId(HttpContext, _db);
        return Json(_db.Scales.Where(s => s.Active).ForSite(siteId)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Name)
            .Select(s => new { s.Id, s.Name, simulated = s.HardwareId == null || s.HardwareId == "" }).ToList());
    }

    [HttpPost("api/scales")]
    public IActionResult AddScale([FromBody] Models.Scale scale)
    {
        if (_db.Scales.Count() >= MaxScales)
            return BadRequest(new { error = $"Maximum of {MaxScales} scales (MaxScales in appsettings.json). Delete or rename one instead." });

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
        existing.SiteId = scale.SiteId;
        existing.SortOrder = scale.SortOrder;
        existing.Active = scale.Active;
        existing.InboundPrinterId = string.IsNullOrWhiteSpace(scale.InboundPrinterId) ? null : scale.InboundPrinterId;
        existing.OutboundPrinterId = string.IsNullOrWhiteSpace(scale.OutboundPrinterId) ? null : scale.OutboundPrinterId;
        _db.SaveChanges();
        return Json(existing);
    }

    // ===== Locations (physical weighing sites) =====
    // Each location has its own scales; operators pick their location in the
    // navbar and only see that location's scales/bins/commodities. Managed on
    // this page because sites and scales are configured together.

    [HttpGet("api/sites")]
    public IActionResult GetSites() =>
        Json(_db.Sites.OrderBy(s => s.SortOrder).ThenBy(s => s.Name)
            .Select(s => new { s.Id, s.Name, s.Address, s.City, s.State, s.Zip, s.Phone, s.Notes, s.SortOrder, s.Active })
            .ToList());

    [HttpPost("api/sites")]
    public IActionResult AddSite([FromBody] Site site)
    {
        var error = ValidateSite(site, null);
        if (error != null) return BadRequest(new { error });

        site.Id = 0;
        site.Name = site.Name.Trim();
        _db.Sites.Add(site);
        _db.SaveChanges();
        return Json(site);
    }

    [HttpPut("api/sites/{id}")]
    public IActionResult UpdateSite(int id, [FromBody] Site site)
    {
        var existing = _db.Sites.Find(id);
        if (existing == null) return NotFound();

        var error = ValidateSite(site, id);
        if (error != null) return BadRequest(new { error });

        existing.Name = site.Name.Trim();
        existing.Address = Clean(site.Address);
        existing.City = Clean(site.City);
        existing.State = Clean(site.State);
        existing.Zip = Clean(site.Zip);
        existing.Phone = Clean(site.Phone);
        existing.Notes = Clean(site.Notes);
        existing.SortOrder = site.SortOrder;
        existing.Active = site.Active;
        _db.SaveChanges();
        return Json(existing);

        static string? Clean(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
    }

    /// <summary>Deleting a location unassigns its scales/bins/commodities
    /// (they become available everywhere) rather than deleting them.</summary>
    [HttpDelete("api/sites/{id}")]
    public IActionResult DeleteSite(int id)
    {
        var existing = _db.Sites.Find(id);
        if (existing == null) return NotFound();

        foreach (var s in _db.Scales.Where(s => s.SiteId == id)) s.SiteId = null;
        foreach (var b in _db.Bins.Where(b => b.SiteId == id)) b.SiteId = null;
        foreach (var c in _db.Commodities.Where(c => c.SiteId == id)) c.SiteId = null;
        _db.Sites.Remove(existing);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    private string? ValidateSite(Site site, int? id)
    {
        if (string.IsNullOrWhiteSpace(site.Name)) return "Location name is required.";
        if (site.Name.Trim().Length > 100) return "Location name must be 100 characters or fewer.";
        if (_db.Sites.Any(s => s.Id != id && s.Name.ToLower() == site.Name.Trim().ToLower()))
            return "A location with that name already exists.";
        return null;
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
