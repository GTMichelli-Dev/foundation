using System.ComponentModel.DataAnnotations;

namespace BasicWeigh.Web.Models;

public class Customer
{
    [Key]
    [StringLength(50)]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
}
