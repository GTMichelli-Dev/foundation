using System.Drawing;
using DevExpress.XtraReports.UI;

namespace BasicWeigh.Web.Reports;

public class TicketReport : XtraReport
{
    public TicketReport()
    {
        // 3" receipt paper (76mm = ~288 pixels at 96dpi, or 3 inches = 300 hundredths)
        PaperKind = (DevExpress.Drawing.Printing.DXPaperKind)System.Drawing.Printing.PaperKind.Custom;
        PageWidth = 300;  // 3 inches in hundredths of an inch
        PageHeight = 800; // tall enough for content
        Margins = new System.Drawing.Printing.Margins(10, 10, 10, 10);
        Font = new Font("Courier New", 9f);
        RollPaper = true;

        // Parameters for ticket data
        AddParameter("Ticket", typeof(string));
        AddParameter("DateIn", typeof(string));
        AddParameter("DateOut", typeof(string));
        AddParameter("Customer", typeof(string));
        AddParameter("Carrier", typeof(string));
        AddParameter("TruckId", typeof(string));
        AddParameter("Commodity", typeof(string));
        AddParameter("Location", typeof(string));
        AddParameter("Destination", typeof(string));
        AddParameter("GrossWeight", typeof(string));
        AddParameter("TareWeight", typeof(string));
        AddParameter("NetWeight", typeof(string));
        AddParameter("Comment", typeof(string));
        AddParameter("Notes", typeof(string));
        AddParameter("IsVoid", typeof(bool));
        AddParameter("Header1", typeof(string));
        AddParameter("Header2", typeof(string));
        AddParameter("Header3", typeof(string));
        AddParameter("Header4", typeof(string));

        // Build the report layout
        var detail = new DetailBand { HeightF = 0, Name = "Detail" };
        Bands.Add(detail);

        var reportHeader = new ReportHeaderBand { HeightF = 0, Name = "ReportHeader" };
        Bands.Add(reportHeader);

        float y = 0;

        // Header lines (centered, bold)
        y = AddCenteredLabel(reportHeader, "Header1", y, 12f, true);
        y = AddCenteredLabel(reportHeader, "Header2", y, 10f, true);
        y = AddCenteredLabel(reportHeader, "Header3", y, 10f, true);
        y = AddCenteredLabel(reportHeader, "Header4", y, 10f, true);

        // Separator
        y += 5;
        var sep1 = CreateLine(y);
        reportHeader.Controls.Add(sep1);
        y += 5;

        // Detail rows
        y = AddLabelRow(reportHeader, "Ticket #:", "Ticket", y);
        y = AddLabelRow(reportHeader, "Date In:", "DateIn", y);
        y = AddLabelRow(reportHeader, "Date Out:", "DateOut", y);
        y = AddLabelRow(reportHeader, "Customer:", "Customer", y);
        y = AddLabelRow(reportHeader, "Carrier:", "Carrier", y);
        y = AddLabelRow(reportHeader, "Truck ID:", "TruckId", y);
        y = AddLabelRow(reportHeader, "Commodity:", "Commodity", y);
        y = AddLabelRow(reportHeader, "Location:", "Location", y);
        y = AddLabelRow(reportHeader, "Destination:", "Destination", y);

        // Weight separator
        y += 3;
        var sep2 = CreateLine(y);
        reportHeader.Controls.Add(sep2);
        y += 5;

        // Weights
        y = AddLabelRow(reportHeader, "Gross Weight:", "GrossWeight", y, 10f);
        y = AddLabelRow(reportHeader, "Tare Weight:", "TareWeight", y, 10f);
        y = AddLabelRow(reportHeader, "Net Weight:", "NetWeight", y, 13f, true);

        // Weight separator
        y += 3;
        var sep3 = CreateLine(y);
        reportHeader.Controls.Add(sep3);
        y += 5;

        // Comment & Notes
        y = AddLabelRow(reportHeader, "Comment:", "Comment", y);
        y = AddLabelRow(reportHeader, "Notes:", "Notes", y);

        // Void stamp
        y += 10;
        var voidLabel = new XRLabel
        {
            Name = "lblVoid",
            Text = "*** VOID ***",
            LocationF = new PointF(0, y),
            SizeF = new SizeF(280, 30),
            Font = new Font("Courier New", 18f, FontStyle.Bold),
            ForeColor = Color.Red,
            TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        };
        voidLabel.ExpressionBindings.Add(
            new ExpressionBinding("BeforePrint", "Visible", "[Parameters.IsVoid]"));
        reportHeader.Controls.Add(voidLabel);
        y += 30;

        reportHeader.HeightF = y;
    }

    private void AddParameter(string name, Type type)
    {
        var param = new DevExpress.XtraReports.Parameters.Parameter
        {
            Name = name,
            Type = type,
            Visible = false
        };
        Parameters.Add(param);
    }

    private float AddCenteredLabel(Band band, string paramName, float y, float fontSize = 9f, bool bold = false)
    {
        var label = new XRLabel
        {
            Name = "lbl" + paramName,
            LocationF = new PointF(0, y),
            SizeF = new SizeF(280, 18),
            Font = new Font("Courier New", fontSize, bold ? FontStyle.Bold : FontStyle.Regular),
            TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        };
        label.ExpressionBindings.Add(
            new ExpressionBinding("BeforePrint", "Text", $"[Parameters.{paramName}]"));
        band.Controls.Add(label);
        return y + 18;
    }

    private float AddLabelRow(Band band, string caption, string paramName, float y, float fontSize = 9f, bool bold = false)
    {
        var captionLabel = new XRLabel
        {
            Name = "cap" + paramName,
            Text = caption,
            LocationF = new PointF(0, y),
            SizeF = new SizeF(110, 16),
            Font = new Font("Courier New", fontSize, bold ? FontStyle.Bold : FontStyle.Regular)
        };
        band.Controls.Add(captionLabel);

        var valueLabel = new XRLabel
        {
            Name = "val" + paramName,
            LocationF = new PointF(110, y),
            SizeF = new SizeF(170, 16),
            Font = new Font("Courier New", fontSize, bold ? FontStyle.Bold : FontStyle.Regular)
        };
        valueLabel.ExpressionBindings.Add(
            new ExpressionBinding("BeforePrint", "Text", $"[Parameters.{paramName}]"));
        band.Controls.Add(valueLabel);

        return y + 16;
    }

    private XRLine CreateLine(float y)
    {
        return new XRLine
        {
            Name = "line" + y.ToString("F0"),
            LocationF = new PointF(0, y),
            SizeF = new SizeF(280, 2),
            LineStyle = (DevExpress.Drawing.DXDashStyle)System.Drawing.Drawing2D.DashStyle.Dash
        };
    }
}
