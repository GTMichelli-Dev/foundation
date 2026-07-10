using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// Admin-defined extra ticket field (Setup → Fields). Rendered on the
/// Weigh In / Weigh Out / Edit / Basic Ticket forms, shown as a grid column,
/// and printed on tickets when ShowOnTicket is set.
/// </summary>
public class CustomField
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Field Name")]
    public string Name { get; set; } = "";

    // Text, Integer, Real
    [StringLength(10)]
    [Display(Name = "Type")]
    public string FieldType { get; set; } = "Text";

    /// <summary>When true the weigh forms refuse to save without a value.</summary>
    [Display(Name = "Required")]
    public bool Required { get; set; }

    /// <summary>Inactive fields stay in history but stop appearing on forms.</summary>
    [Display(Name = "Active")]
    public bool Active { get; set; } = true;

    [Display(Name = "Show on Ticket")]
    public bool ShowOnTicket { get; set; } = true;

    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }

    /// <summary>
    /// Text fields only: newline-separated dropdown choices. When set, the
    /// weigh forms render a select instead of a free-text input. Managed on
    /// Setup → Fields (one value per line = full add/edit/delete/reorder).
    /// </summary>
    [StringLength(4000)]
    public string? ListValues { get; set; }

    /// <summary>Integer/Real fields only: inclusive minimum allowed value.</summary>
    public double? MinValue { get; set; }

    /// <summary>Integer/Real fields only: inclusive maximum allowed value.</summary>
    public double? MaxValue { get; set; }

    /// <summary>Real fields only: max decimal places (0–6). Null = unlimited.</summary>
    public int? Precision { get; set; }

    /// <summary>
    /// Ask for this field during the kiosk weigh-in flow. Only meaningful for
    /// constrained inputs — numeric fields and list-backed text fields; a
    /// free-text field can't prompt at the kiosk (no keyboard-driven prose).
    /// </summary>
    public bool PromptAtKiosk { get; set; }

    /// <summary>True when this field is allowed to prompt at the kiosk.</summary>
    public bool IsKioskEligible() =>
        FieldType != "Text" || GetListValues().Count > 0;

    /// <summary>ListValues split into clean entries (empty for free-text fields).</summary>
    public List<string> GetListValues() =>
        (ListValues ?? "")
            .Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToList();
}

/// <summary>Per-ticket value for a CustomField. Stored as text; the field's
/// FieldType drives input rendering and validation, not storage.</summary>
public class TransactionCustomValue
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Ticket { get; set; } = "";

    public int CustomFieldId { get; set; }

    [StringLength(200)]
    public string? Value { get; set; }
}
