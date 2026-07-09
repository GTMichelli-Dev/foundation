using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    [Display(Name = "Use at Kiosk")]
    public bool UseAtKiosk { get; set; } = true;
}
