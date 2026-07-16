using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.UI;
using Foundation.Web.Data;
using Foundation.Web.Reports;

namespace Foundation.Web.Services;

public class ReportStorageService : ReportStorageWebExtension
{
    private readonly string _reportsDir;
    private readonly IServiceProvider? _services;

    /// <param name="services">Root service provider used to resolve a scoped
    /// ScaleDbContext per GetData call (this extension is registered globally,
    /// outside DI). Null skips custom-field parameter injection.</param>
    public ReportStorageService(IServiceProvider? services = null)
    {
        _services = services;
        _reportsDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        if (!Directory.Exists(_reportsDir))
            Directory.CreateDirectory(_reportsDir);
    }

    public override bool CanSetData(string url) => true;

    public override bool IsValidUrl(string url) => true;

    public override byte[] GetData(string url)
    {
        var path = GetPath(url);
        if (!File.Exists(path))
        {
            // No saved .repx yet — generate from the coded report and persist
            // immediately so the file exists on disk after the very first time
            // the designer is opened. Subsequent saves (and the View / KioskView
            // print paths, which also check File.Exists) then read the same file.
            XtraReport? seed = url switch
            {
                "TicketReport" => new TicketReport(),
                "KioskTicketReport" => new KioskTicketReport(),
                _ => null
            };
            if (seed == null) throw new FileNotFoundException($"Report '{url}' not found.");

            seed.SaveLayoutToXml(path);
            Console.WriteLine($"[ReportStorage] seed '{url}' to {path}");
        }

        Console.WriteLine($"[ReportStorage] load '{url}' from {path}");
        return WithCustomFieldParameters(path);
    }

    /// <summary>
    /// Serve the saved layout with a cf_ parameter for every active custom
    /// field, so the fields appear in the designer's Field List — including
    /// fields with Show-on-Ticket unchecked, which only print when placed in
    /// the layout here (checked fields auto-append instead). In-memory only —
    /// the .repx on disk is untouched until the user saves (at which point
    /// any placed parameters persist with it).
    /// </summary>
    private byte[] WithCustomFieldParameters(string path)
    {
        var bytes = File.ReadAllBytes(path);
        if (_services == null) return bytes;

        try
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ScaleDbContext>();
            var fields = db.CustomFields
                .Where(f => f.Active)
                .OrderBy(f => f.SortOrder).ThenBy(f => f.Name)
                .ToList();
            if (fields.Count == 0) return bytes;

            using var input = new MemoryStream(bytes);
            var report = new XtraReport();
            report.LoadLayoutFromXml(input);
            foreach (var field in fields)
                CustomFieldParams.EnsureParameter(report, CustomFieldParams.ParamName(field.Name), field.Name);

            using var output = new MemoryStream();
            report.SaveLayoutToXml(output);
            return output.ToArray();
        }
        catch (Exception ex)
        {
            // Never block the designer over parameter injection
            Console.WriteLine($"[ReportStorage] custom-field parameter injection failed: {ex.Message}");
            return bytes;
        }
    }

    public override void SetData(XtraReport report, string url)
    {
        var path = GetPath(url);
        report.SaveLayoutToXml(path);
        Console.WriteLine($"[ReportStorage] save '{url}' to {path}");
    }

    public override string SetNewData(XtraReport report, string defaultUrl)
    {
        var url = string.IsNullOrEmpty(defaultUrl) ? "NewReport" : defaultUrl;
        SetData(report, url);
        return url;
    }

    public override Dictionary<string, string> GetUrls()
    {
        var urls = new Dictionary<string, string>();

        // Always show built-in reports
        urls["TicketReport"] = "Ticket Report";
        urls["KioskTicketReport"] = "Kiosk Inbound Ticket";

        // Add any other .repx files in the folder
        foreach (var file in Directory.GetFiles(_reportsDir, "*.repx"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (!urls.ContainsKey(name))
                urls[name] = name;
        }

        return urls;
    }

    private string GetPath(string url)
    {
        var safeName = Path.GetFileNameWithoutExtension(url);
        return Path.Combine(_reportsDir, safeName + ".repx");
    }
}
