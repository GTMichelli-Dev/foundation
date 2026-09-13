using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// Decides whether a ticket prints by itself when a truck is weighed
/// (Setup → Printing). Rules are checked in order and the first one whose
/// conditions all match wins; a blank condition matches anything. When none
/// match, AppSetup.PrintWhenNoRuleMatches decides. Reprints are never subject
/// to rules — someone asked for that ticket.
/// </summary>
public class PrintRule
{
    public const string Any = "Any";
    public const string SourceKiosk = "Kiosk";
    public const string SourceOffice = "Office";
    public const string LegInbound = "Inbound";
    public const string LegOutbound = "Outbound";

    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    public bool Active { get; set; } = true;

    /// <summary>Checked lowest first.</summary>
    public int SortOrder { get; set; }

    /// <summary>Where the ticket is weighed: "Any", "Kiosk" or "Office" (the
    /// weigh forms).</summary>
    [StringLength(10)]
    public string Source { get; set; } = Any;

    /// <summary>Which ticket: "Any", "Inbound" (still open after this
    /// weighment) or "Outbound" (completed).</summary>
    [StringLength(10)]
    public string Leg { get; set; } = Any;

    [StringLength(50)]
    public string? Customer { get; set; }

    [StringLength(50)]
    public string? Carrier { get; set; }

    [StringLength(50)]
    public string? TruckId { get; set; }

    [StringLength(50)]
    public string? Commodity { get; set; }

    [StringLength(50)]
    public string? Location { get; set; }

    [StringLength(50)]
    public string? Destination { get; set; }

    /// <summary>Optional custom-field condition: this field must hold
    /// <see cref="CustomFieldValue"/>.</summary>
    public int? CustomFieldId { get; set; }

    [StringLength(200)]
    public string? CustomFieldValue { get; set; }

    /// <summary>True prints the ticket, false keeps it from printing.</summary>
    public bool Print { get; set; }
}
