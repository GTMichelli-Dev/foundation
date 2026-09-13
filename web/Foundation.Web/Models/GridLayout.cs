using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// The shared layout of an operator grid (Trucks in Yard, Completed): column
/// order, widths, visibility and sort, as DevExtreme's state object. A Manager
/// or Admin arranges it and every user sees it; no row means the built-in
/// default. Filters and paging position are never stored — those stay with
/// whoever set them.
/// </summary>
public class GridLayout
{
    [Key]
    [StringLength(50)]
    public string GridKey { get; set; } = "";

    public string StateJson { get; set; } = "";

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}
