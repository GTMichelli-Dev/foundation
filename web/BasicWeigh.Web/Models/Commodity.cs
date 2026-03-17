using System.ComponentModel.DataAnnotations;

namespace BasicWeigh.Web.Models;

public class Commodity
{
    [Key]
    [StringLength(50)]
    [Display(Name = "Commodity Name")]
    public string CommodityName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
}
