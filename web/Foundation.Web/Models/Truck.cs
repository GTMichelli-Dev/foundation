using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

public class Truck
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Truck ID")]
    public string TruckId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Carrier")]
    public string CarrierName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [Display(Name = "Use at Kiosk")]
    public bool UseAtKiosk { get; set; } = true;

    /// <summary>
    /// Last known empty (tare) weight for this truck. When set, a kiosk weigh-in
    /// will be auto-completed using this value instead of leaving the ticket open
    /// for a second weigh.
    /// </summary>
    [Display(Name = "Retained Tare")]
    public int? RetainedTare { get; set; }

    /// <summary>
    /// When the retained tare was last refreshed (typically the DateOut of the
    /// transaction that produced it).
    /// </summary>
    [Display(Name = "Tare Updated")]
    public DateTime? RetainedTareUpdated { get; set; }
}
