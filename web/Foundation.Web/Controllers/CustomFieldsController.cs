using Microsoft.AspNetCore.Mvc;
using Foundation.Web.Data;
using Foundation.Web.Models;

namespace Foundation.Web.Controllers;

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
            f.Id, f.Name, f.FieldType, f.Required, f.Active, f.ShowOnTicket, f.SortOrder,
            f.ListValues, f.MinValue, f.MaxValue, f.Precision
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
        Normalize(field);
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

        Normalize(field);
        existing.Name = field.Name.Trim();
        existing.FieldType = field.FieldType;
        existing.Required = field.Required;
        existing.Active = field.Active;
        existing.ShowOnTicket = field.ShowOnTicket;
        existing.SortOrder = field.SortOrder;
        existing.ListValues = field.ListValues;
        existing.MinValue = field.MinValue;
        existing.MaxValue = field.MaxValue;
        existing.Precision = field.Precision;
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
        if (field.MinValue.HasValue && field.MaxValue.HasValue && field.MinValue > field.MaxValue)
            return "Min must be less than or equal to Max.";
        if (field.Precision is < 0 or > 6)
            return "Decimals must be between 0 and 6.";
        if (field.FieldType == "Text" && field.ListValues != null)
        {
            var values = field.GetListValues();
            if (values.Any(v => v.Length > 200))
                return "Each dropdown value must be 200 characters or fewer.";
            if (string.Join("\n", values).Length > 4000)
                return "Dropdown list is too long (4000 characters max).";
        }
        return null;
    }

    /// <summary>Clears settings that don't apply to the chosen type, and
    /// canonicalizes the dropdown list (trimmed lines, blanks dropped).</summary>
    private static void Normalize(CustomField field)
    {
        if (field.FieldType == "Text")
        {
            var values = field.GetListValues();
            field.ListValues = values.Count > 0 ? string.Join("\n", values) : null;
            field.MinValue = null;
            field.MaxValue = null;
            field.Precision = null;
        }
        else
        {
            field.ListValues = null;
            if (field.FieldType == "Integer") field.Precision = null;
        }
    }
}
