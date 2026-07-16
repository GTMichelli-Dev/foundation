using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// A physical weighing location (yard/facility) with its own scales. Shown as
/// "Location" in the UI — distinct from the Location ticket field, which is
/// where a load came from. Operators pick their location in the navbar
/// (per-browser cookie); the weigh forms, Get Weight tiles, and kiosks then
/// only offer the scales — and optionally bins/commodities — assigned to it.
/// Scales/bins/commodities with no SiteId are available at every location.
/// </summary>
public class Site
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Location Name")]
    public string Name { get; set; } = "";

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(20)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? Zip { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }

    /// <summary>Inactive locations disappear from pickers; their assignments stay.</summary>
    public bool Active { get; set; } = true;
}
