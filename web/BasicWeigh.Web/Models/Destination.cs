using System.ComponentModel.DataAnnotations;

namespace BasicWeigh.Web.Models;

public class Destination
{
    [Key]
    [StringLength(50)]
    [Display(Name = "Destination Name")]
    public string DestinationName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
}
