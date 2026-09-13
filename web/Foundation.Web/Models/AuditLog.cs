using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// One change to one field of one record, written by ScaleDbContext whenever
/// it saves (see Services/AuditTrail). A record created or deleted is a single
/// row carrying a summary of its values instead of one row per field.
/// </summary>
public class AuditLog
{
    [Key]
    public long Id { get; set; }

    /// <summary>When the change was saved, UTC.</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>The signed-in user. Null when Require Login is off, and for
    /// kiosks, phones without a login and background services.</summary>
    [StringLength(100)]
    public string? UserName { get; set; }

    /// <summary>Where the change came from: Office, Kiosk (with the kiosk's
    /// name when known), Phone, Signature Pad, API, Device Service, System.</summary>
    [StringLength(100)]
    public string Source { get; set; } = "";

    /// <summary>The kind of record: Ticket, Customer, Card, Settings, …</summary>
    [StringLength(50)]
    public string EntityType { get; set; } = "";

    /// <summary>Which record: a ticket number, a customer's name, a card
    /// number, …</summary>
    [StringLength(100)]
    public string EntityKey { get; set; } = "";

    /// <summary>Created, Changed or Deleted.</summary>
    [StringLength(10)]
    public string Action { get; set; } = "";

    /// <summary>The field that changed. Null on Created / Deleted rows.</summary>
    [StringLength(100)]
    public string? Field { get; set; }

    [StringLength(1000)]
    public string? OldValue { get; set; }

    [StringLength(1000)]
    public string? NewValue { get; set; }
}
