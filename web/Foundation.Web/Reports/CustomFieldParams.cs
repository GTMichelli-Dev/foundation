using System.Text.RegularExpressions;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;

namespace Foundation.Web.Reports;

/// <summary>
/// Bridges admin-defined custom fields (Setup → Fields) and the DevExpress
/// ticket reports. Each active Show-on-Ticket field is exposed as a report
/// parameter named cf_&lt;SanitizedName&gt; so it can be placed in the designer;
/// fields not referenced by the layout keep the legacy auto-appended row
/// (see TicketController.InjectCustomFields).
/// </summary>
public static class CustomFieldParams
{
    /// <summary>Report-parameter name for a custom field: "cf_" prefix,
    /// non-alphanumerics collapsed to underscores ("Lot #" → cf_Lot__).</summary>
    public static string ParamName(string fieldName) =>
        "cf_" + Regex.Replace(fieldName, "[^A-Za-z0-9]", "_");

    /// <summary>Add the parameter if the report doesn't have it yet. Hidden
    /// from the viewer's parameters panel (like the built-in ticket params);
    /// the Description carries the admin-facing field name for the Field List.</summary>
    public static void EnsureParameter(XtraReport report, string paramName, string displayName)
    {
        if (report.Parameters[paramName] != null) return;
        report.Parameters.Add(new Parameter
        {
            Name = paramName,
            Description = displayName,
            Type = typeof(string),
            Value = "",
            Visible = false
        });
    }

    /// <summary>
    /// True when any control in the layout references the parameter — in an
    /// expression binding ("?cf_Foo", "[Parameters.cf_Foo]") or embedded in
    /// label text. Word-bounded so cf_Lot doesn't match cf_Lot_2.
    /// </summary>
    public static bool IsReferenced(XtraReport report, string paramName)
    {
        var pattern = new Regex($@"\b{Regex.Escape(paramName)}\b");
        foreach (var control in report.AllControls<XRControl>())
        {
            if (control.Text != null && pattern.IsMatch(control.Text)) return true;
            foreach (ExpressionBinding binding in control.ExpressionBindings)
                if (binding.Expression != null && pattern.IsMatch(binding.Expression)) return true;
        }
        return false;
    }
}
