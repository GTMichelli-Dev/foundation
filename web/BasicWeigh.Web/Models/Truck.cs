using System.ComponentModel.DataAnnotations;

namespace BasicWeigh.Web.Models;

public class Truck
{
    [Key]
    [StringLength(50)]
    [Display(Name = "Truck ID")]
    public string TruckId { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(50)]
    public string? Lot { get; set; }
}
