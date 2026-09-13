using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Foundation.Web.Data;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

/// <summary>
/// The Audit Trail page: every change saved anywhere in the app, who made it
/// and from where (Services/AuditTrail writes the rows). Manager or Admin —
/// the login middleware gates /Audit and /api/audit.
/// </summary>
public class AuditController : Controller
{
    private readonly ScaleDbContext _db;

    /// <summary>Most rows one query returns. A busy site writes a few hundred
    /// a day; narrowing the dates is the way to see further back.</summary>
    private const int MaxRows = 5000;

    public AuditController(ScaleDbContext db) => _db = db;

    /// <summary>With type + key (a ticket's History button) the page opens on
    /// that one record's history, all dates.</summary>
    public IActionResult Index(string? type = null, string? key = null)
    {
        ViewBag.Type = type ?? "";
        ViewBag.Key = key ?? "";
        return View();
    }

    [HttpGet("api/audit")]
    public IActionResult Get(DateTime? startDate, DateTime? endDate, string? type = null, string? key = null)
    {
        var q = _db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(key))
        {
            q = q.Where(a => a.EntityType == type && a.EntityKey == key);
        }
        else
        {
            // Dates arrive as local days in the display time zone, like the
            // Completed grid; the end day is included in full.
            var start = AppTimeZone.ToUtc(startDate ?? DateTime.Today.AddDays(-7));
            var end = AppTimeZone.ToUtc((endDate ?? DateTime.Today).Date.AddDays(1));
            q = q.Where(a => a.Timestamp >= start && a.Timestamp < end);
        }

        var rows = q.OrderByDescending(a => a.Id).Take(MaxRows + 1).ToList();
        var truncated = rows.Count > MaxRows;
        if (truncated) rows.RemoveAt(rows.Count - 1);

        return Json(new
        {
            truncated,
            maxRows = MaxRows,
            rows = rows.Select(a => new
            {
                a.Id,
                Timestamp = a.Timestamp.AsUtc(),
                a.UserName,
                a.Source,
                a.EntityType,
                a.EntityKey,
                a.Action,
                a.Field,
                a.OldValue,
                a.NewValue
            })
        });
    }
}
