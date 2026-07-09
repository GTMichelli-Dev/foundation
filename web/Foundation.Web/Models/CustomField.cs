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
