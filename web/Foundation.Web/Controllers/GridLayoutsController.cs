using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Foundation.Web.Data;
using Foundation.Web.Models;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

/// <summary>
/// Shared layouts for the operator grids. Anyone who can see a grid reads its
/// layout; a Manager or Admin (anyone, with Require Login off) changes it for
/// everybody and can reset it to the built-in default.
/// </summary>
public class GridLayoutsController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly AppSetupCache _setupCache;

    /// <summary>The grids that have a shared layout. Anything else is refused
    /// so the table can't be used as general storage.</summary>
    private static readonly HashSet<string> Grids = new(StringComparer.OrdinalIgnoreCase)
    {
        "completedTrucks", "inboundTrucks"
    };

    private const int MaxStateBytes = 64 * 1024;

    public GridLayoutsController(ScaleDbContext db, AppSetupCache setupCache)
    {
        _db = db;
        _setupCache = setupCache;
    }

    private bool CanEdit()
    {
        if (!_setupCache.Get().UseLogin) return true;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        return role is "Manager" or "Admin";
    }

    [HttpGet("api/grid-layouts/{grid}")]
    public IActionResult Get(string grid)
    {
        if (!Grids.Contains(grid)) return NotFound();
        var layout = _db.GridLayouts.Find(grid);
        JsonElement? state = null;
        if (layout != null)
        {
            try { state = JsonDocument.Parse(layout.StateJson).RootElement.Clone(); }
            catch (JsonException) { state = null; } // unreadable — fall back to the default layout
        }
        return Json(new
        {
            state,
            canEdit = CanEdit(),
            updatedBy = layout?.UpdatedBy,
            updatedAt = layout?.UpdatedAt.AsUtc()
        });
    }

    [HttpPut("api/grid-layouts/{grid}")]
    public IActionResult Save(string grid, [FromBody] JsonElement state)
    {
        if (!Grids.Contains(grid)) return NotFound();
        if (!CanEdit()) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only a Manager or Admin can change the shared layout." });
        if (state.ValueKind != JsonValueKind.Object) return BadRequest(new { message = "Layout must be an object." });

        var json = state.GetRawText();
        if (json.Length > MaxStateBytes) return BadRequest(new { message = "Layout is too large." });

        var layout = _db.GridLayouts.Find(grid);
        if (layout == null)
        {
            layout = new GridLayout { GridKey = grid };
            _db.GridLayouts.Add(layout);
        }
        layout.StateJson = json;
        layout.UpdatedBy = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
        layout.UpdatedAt = DateTime.UtcNow;
        _db.SaveChanges();
        return Ok(new { success = true });
    }

    [HttpDelete("api/grid-layouts/{grid}")]
    public IActionResult Reset(string grid)
    {
        if (!Grids.Contains(grid)) return NotFound();
        if (!CanEdit()) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only a Manager or Admin can reset the shared layout." });

        var layout = _db.GridLayouts.Find(grid);
        if (layout != null)
        {
            _db.GridLayouts.Remove(layout);
            _db.SaveChanges();
        }
        return Ok(new { success = true });
    }
}
