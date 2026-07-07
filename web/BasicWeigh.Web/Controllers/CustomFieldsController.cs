using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Controllers;

/// <summary>
/// CRUD API for admin-defined custom ticket fields (managed on Setup → Fields).
/// </summary>
public class CustomFieldsController : Controller
{
    private static readonly string[] ValidTypes = { "Text", "Integer", "Real" };

    private readonly ScaleDbContext _db;

    public CustomFieldsController(ScaleDbContext db)
    {
        _db = db;
    }

    [HttpGet("api/customfields")]
    public IActionResult GetAll()
    {
        var fields = _db.CustomFields
            .OrderBy(f => f.SortOrder).ThenBy(f => f.Name)
            .ToList();
        return Json(fields.Select(f => new
        {
            f.Id, f.Name, f.FieldType, f.Required, f.Active, f.ShowOnTicket, f.SortOrder
        }));
    }

    [HttpPost("api/customfields")]
    public IActionResult Add([FromBody] CustomField field)
    {
        var error = Validate(field);
        if (error != null) return BadRequest(new { error });
        if (_db.CustomFields.Any(f => f.Name.ToLower() == field.Name.Trim().ToLower()))
            return BadRequest(new { error = "A field with that name already exists." });

        field.Id = 0;
        field.Name = field.Name.Trim();
        _db.CustomFields.Add(field);
        _db.SaveChanges();
        return Json(field);
    }

    [HttpPut("api/customfields/{id}")]
    public IActionResult Update(int id, [FromBody] CustomField field)
    {
        var existing = _db.CustomFields.Find(id);
        if (existing == null) return NotFound();

        var error = Validate(field);
        if (error != null) return BadRequest(new { error });
        if (_db.CustomFields.Any(f => f.Id != id && f.Name.ToLower() == field.Name.Trim().ToLower()))
            return BadRequest(new { error = "A field with that name already exists." });

        existing.Name = field.Name.Trim();
        existing.FieldType = field.FieldType;
        existing.Required = field.Required;
        existing.Active = field.Active;
        existing.ShowOnTicket = field.ShowOnTicket;
        existing.SortOrder = field.SortOrder;
        _db.SaveChanges();
        return Json(existing);
    }

    /// <summary>Deletes the definition AND all stored ticket values (cascade).
    /// To retire a field but keep history, set Active = false instead.</summary>
    [HttpDelete("api/customfields/{id}")]
    public IActionResult Delete(int id)
    {
        var existing = _db.CustomFields.Find(id);
        if (existing == null) return NotFound();

        var valueCount = _db.TransactionCustomValues.Count(v => v.CustomFieldId == id);
        _db.CustomFields.Remove(existing);
        _db.SaveChanges();
        return Json(new { success = true, deletedValues = valueCount });
    }

    private static string? Validate(CustomField field)
    {
        if (string.IsNullOrWhiteSpace(field.Name)) return "Field name is required.";
        if (field.Name.Trim().Length > 50) return "Field name must be 50 characters or fewer.";
        if (!ValidTypes.Contains(field.FieldType)) return "Type must be Text, Integer, or Real.";
        return null;
    }
}
