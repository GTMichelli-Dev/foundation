using System.ComponentModel.DataAnnotations;

namespace BasicWeigh.Web.Models;

public class Location
{
    [Key]
    [StringLength(50)]
    [Display(Name = "Location Name")]
    public string LocationName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
}
