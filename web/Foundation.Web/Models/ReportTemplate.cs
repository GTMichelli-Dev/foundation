using System.ComponentModel.DataAnnotations;

namespace Foundation.Web.Models;

/// <summary>
/// A saved layout of the Reports → Transactions grid (report builder): which
/// columns are visible and in what order, plus grouping, sorting, filters,
/// and widths — the DevExtreme grid state as JSON. Operators arrange the grid
/// how they want, save it under a name, and reload it any time; the standard
/// exports (PDF/Excel/CSV/JSON) then produce that report. Stored server-side
/// so templates are shared by every workstation.
/// </summary>
public class ReportTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    /// <summary>dxDataGrid state (grid.state()) serialized as JSON.</summary>
    public string StateJson { get; set; } = "{}";

    public DateTime Updated { get; set; }
}
