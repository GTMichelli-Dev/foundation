using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// A storage bin for the Bin Inventory feature (Setup → Options →
/// Use Bin Inventory). Loads delivered into a bin add to its inventory,
/// loads hauled out deduct — direction is inferred from the ticket weights.
/// </summary>
public class Bin
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Bin Name")]
    public string BinName { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    [Display(Name = "Use at Kiosk")]
    public bool UseAtKiosk { get; set; } = true;
}
