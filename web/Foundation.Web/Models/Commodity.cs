using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

public class Commodity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Commodity Name")]
    public string CommodityName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    [Display(Name = "Use at Kiosk")]
    public bool UseAtKiosk { get; set; } = true;

    /// <summary>Optional physical location (Site) this commodity is handled
    /// at; null = offered at every location.</summary>
    [Display(Name = "Location")]
    public int? SiteId { get; set; }
}
