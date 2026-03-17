using System.ComponentModel.DataAnnotations;

namespace BasicWeigh.Web.Models;

public class Carrier
{
    [Key]
    [StringLength(50)]
    [Display(Name = "Carrier Name")]
    public string CarrierName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
}
