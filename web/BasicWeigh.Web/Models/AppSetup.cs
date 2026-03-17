using System.ComponentModel.DataAnnotations;

namespace BasicWeigh.Web.Models;

public class AppSetup
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Next Ticket Number")]
    public int TicketNumber { get; set; } = 1;

    [StringLength(100)]
    [Display(Name = "Header Line 1")]
    public string? Header1 { get; set; }

    [StringLength(100)]
    [Display(Name = "Header Line 2")]
    public string? Header2 { get; set; }

    [StringLength(100)]
    [Display(Name = "Header Line 3")]
    public string? Header3 { get; set; }

    [StringLength(100)]
    [Display(Name = "Header Line 4")]
    public string? Header4 { get; set; }

    [StringLength(100)]
    [Display(Name = "Printer Name")]
    public string? PrinterName { get; set; }

    [Display(Name = "Tickets Per Page")]
    public int TicketsPerPage { get; set; } = 1;
}
