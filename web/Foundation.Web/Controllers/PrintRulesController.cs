using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Foundation.Web.Data;
using Foundation.Web.Models;

namespace Foundation.Web.Controllers;

/// <summary>
/// Print rules for Setup → Printing. Admin-only, like the rest of Setup (the
/// login middleware gates /api/printrules). See Services/PrintRules for how
/// they are applied.
/// </summary>
public class PrintRulesController : Controller
{
    private readonly ScaleDbContext _db;

    public PrintRulesController(ScaleDbContext db) => _db = db;

    [HttpGet("api/printrules")]
    public IActionResult List() => Json(_db.PrintRules.AsNoTracking()
        .OrderBy(r => r.SortOrder).ThenBy(r => r.Id).ToList());

    /// <summary>What each condition can be set to — the same lists the weigh
    /// forms offer, so a rule can only name a value a ticket could carry.</summary>
    [HttpGet("api/printrules/options")]
    public IActionResult Options() => Json(new
    {
        customers = _db.Customers.Where(c => c.Active).OrderBy(c => c.CustomerName).Select(c => c.CustomerName).ToList(),
        carriers = _db.Carriers.Where(c => c.Active).OrderBy(c => c.CarrierName).Select(c => c.CarrierName).ToList(),
        trucks = _db.Trucks.Select(t => t.TruckId).Distinct().OrderBy(t => t).ToList(),
        commodities = _db.Commodities.Where(c => c.Active).OrderBy(c => c.CommodityName).Select(c => c.CommodityName).ToList(),
        locations = _db.Locations.Where(l => l.Active).OrderBy(l => l.LocationName).Select(l => l.LocationName).ToList(),
        destinations = _db.Destinations.Where(d => d.Active).OrderBy(d => d.DestinationName).Select(d => d.DestinationName).ToList(),
        customFields = _db.CustomFields.Where(f => f.Active && f.FieldType != "Formula")
            .OrderBy(f => f.SortOrder).ThenBy(f => f.Name)
            .Select(f => new { id = f.Id, name = f.Name }).ToList()
    });

    [HttpPost("api/printrules")]
    public IActionResult Create([FromBody] PrintRule rule)
    {
        rule.Id = 0;
        if (Clean(rule) is { } error) return BadRequest(new { message = error });
        if (rule.SortOrder == 0)
            rule.SortOrder = (_db.PrintRules.Max(r => (int?)r.SortOrder) ?? 0) + 10;
        _db.PrintRules.Add(rule);
        _db.SaveChanges();
        return Json(rule);
    }

    [HttpPut("api/printrules/{id:int}")]
    public IActionResult Update(int id, [FromBody] PrintRule rule)
    {
        var existing = _db.PrintRules.Find(id);
        if (existing == null) return NotFound();
        if (Clean(rule) is { } error) return BadRequest(new { message = error });

        existing.Name = rule.Name;
        existing.Active = rule.Active;
        existing.SortOrder = rule.SortOrder;
        existing.Source = rule.Source;
        existing.Leg = rule.Leg;
        existing.Customer = rule.Customer;
        existing.Carrier = rule.Carrier;
        existing.TruckId = rule.TruckId;
        existing.Commodity = rule.Commodity;
        existing.Location = rule.Location;
        existing.Destination = rule.Destination;
        existing.CustomFieldId = rule.CustomFieldId;
        existing.CustomFieldValue = rule.CustomFieldValue;
        existing.Print = rule.Print;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/printrules/{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = _db.PrintRules.Find(id);
        if (existing == null) return NotFound();
        _db.PrintRules.Remove(existing);
        _db.SaveChanges();
        return Ok(new { success = true });
    }

    /// <summary>Normalise a posted rule in place; returns an error message or
    /// null. Blank conditions are stored as null so they match anything.</summary>
    private string? Clean(PrintRule r)
    {
        r.Name = (r.Name ?? "").Trim();
        if (r.Name.Length == 0) return "Give the rule a name.";
        if (r.Name.Length > 100) r.Name = r.Name[..100];

        r.Source = Pick(r.Source, PrintRule.Any, PrintRule.SourceKiosk, PrintRule.SourceOffice);
        r.Leg = Pick(r.Leg, PrintRule.Any, PrintRule.LegInbound, PrintRule.LegOutbound);

        r.Customer = Blank(r.Customer, 50);
        r.Carrier = Blank(r.Carrier, 50);
        r.TruckId = Blank(r.TruckId, 50);
        r.Commodity = Blank(r.Commodity, 50);
        r.Location = Blank(r.Location, 50);
        r.Destination = Blank(r.Destination, 50);
        r.CustomFieldValue = Blank(r.CustomFieldValue, 200);

        if (r.CustomFieldId.HasValue && !_db.CustomFields.Any(f => f.Id == r.CustomFieldId.Value))
            return "That custom field no longer exists.";
        if (r.CustomFieldId.HasValue && r.CustomFieldValue == null)
            return "Enter the value the custom field must hold, or clear the field.";
        if (!r.CustomFieldId.HasValue) r.CustomFieldValue = null;
        return null;
    }

    private static string Pick(string? value, params string[] allowed) =>
        allowed.FirstOrDefault(a => a.Equals(value?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? allowed[0];

    private static string? Blank(string? value, int max)
    {
        var v = value?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        return v.Length > max ? v[..max] : v;
    }
}
