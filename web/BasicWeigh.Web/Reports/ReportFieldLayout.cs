using System.Drawing;
using DevExpress.Drawing;
using DevExpress.XtraReports.UI;

namespace BasicWeigh.Web.Reports;

/// <summary>
/// Render-time layout surgery on the ticket reports so hidden fields leave no
/// blank gaps and custom fields get their own printed rows. Works on both the
/// code-default reports and designer-edited .repx templates, as long as the
/// cap{Param}/val{Param} control names are kept; missing controls are no-ops.
/// </summary>
public static class ReportFieldLayout
{
    private const float Epsilon = 0.1f;

    /// <summary>
    /// Collapse the cap{param}/val{param} row of a hidden field: hide both
    /// controls and shift everything below them up by the freed height.
    /// </summary>
    public static void CollapseRow(XtraReport report, string paramName)
    {
        var cap = report.FindControl("cap" + paramName, true) as XRControl;
        var val = report.FindControl("val" + paramName, true) as XRControl;
        var anchor = cap ?? val;
        if (anchor?.Band == null) return;

        float top = anchor.LocationF.Y;
        float height = anchor.HeightF;
        if (cap != null) cap.Visible = false;
        if (val != null) val.Visible = false;

        ShiftControls(anchor.Band, top, -height, includeAt: false);
    }

    /// <summary>Collapse arbitrary named controls (e.g. the signature block
    /// when no signature exists) the same way.</summary>
    public static void Collapse(XtraReport report, params string[] controlNames)
    {
        foreach (var name in controlNames)
        {
            var c = report.FindControl(name, true) as XRControl;
            if (c?.Band == null) continue;
            c.Visible = false;
            ShiftControls(c.Band, c.LocationF.Y, -c.HeightF, includeAt: false);
        }
    }

    /// <summary>
    /// Insert caption/value rows above the first control found among
    /// anchorNames, pushing the anchor and everything below it down.
    /// Falls back to appending at the bottom of the report header band.
    /// </summary>
    public static void InsertRows(XtraReport report, List<(string Label, string Value)> rows, params string[] anchorNames)
    {
        if (rows.Count == 0) return;

        XRControl? anchor = null;
        foreach (var name in anchorNames)
        {
            anchor = report.FindControl(name, true) as XRControl;
            if (anchor?.Band != null) break;
            anchor = null;
        }

        var band = anchor?.Band ?? report.Bands.OfType<ReportHeaderBand>().FirstOrDefault() as Band;
        if (band == null) return;

        const float rowHeight = 16f;
        float insertY = anchor?.LocationF.Y ?? band.HeightF;
        float delta = rows.Count * rowHeight + 3;

        ShiftControls(band, insertY, delta, includeAt: true);

        var font = new DXFont("Courier New", 9f);
        float y = insertY;
        foreach (var (label, value) in rows)
        {
            band.Controls.Add(new XRLabel
            {
                Text = label,
                LocationF = new PointF(0, y),
                SizeF = new SizeF(110, rowHeight),
                Font = font
            });
            band.Controls.Add(new XRLabel
            {
                Text = value,
                LocationF = new PointF(110, y),
                SizeF = new SizeF(170, rowHeight),
                Font = font
            });
            y += rowHeight;
        }
    }

    private static void ShiftControls(Band band, float belowY, float delta, bool includeAt)
    {
        foreach (XRControl c in band.Controls)
        {
            var y = c.LocationF.Y;
            var affected = includeAt ? y >= belowY - Epsilon : y > belowY + Epsilon;
            if (affected)
                c.LocationF = new PointF(c.LocationF.X, y + delta);
        }
        band.HeightF = Math.Max(0, band.HeightF + delta);
    }
}
