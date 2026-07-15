using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// A named site scale (Scale page), grain-management style: operators pick a
/// scale by name on the weigh forms and kiosks, and tickets record which scale
/// captured each weighment. HardwareId links the scale to a live indicator
/// feed ("serviceId:scaleId" as reported by a ScaleReaderService); a scale
/// with no HardwareId is driven by the per-scale simulator in demo mode.
/// </summary>
public class Scale
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Scale Name")]
    public string Name { get; set; } = "";

    /// <summary>"serviceId:scaleId" of the indicator feed, null = simulated.</summary>
    [StringLength(100)]
    [Display(Name = "Hardware Feed")]
    public string? HardwareId { get; set; }

    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }

    /// <summary>Inactive scales disappear from pickers but stay on old tickets.</summary>
    [Display(Name = "Active")]
    public bool Active { get; set; } = true;
}
