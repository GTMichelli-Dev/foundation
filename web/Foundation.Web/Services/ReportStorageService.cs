using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.UI;
using Foundation.Web.Reports;

namespace Foundation.Web.Services;

public class ReportStorageService : ReportStorageWebExtension
{
    private readonly string _reportsDir;

    public ReportStorageService()
    {
        _reportsDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        if (!Directory.Exists(_reportsDir))
            Directory.CreateDirectory(_reportsDir);
    }

    public override bool CanSetData(string url) => true;

    public override bool IsValidUrl(string url) => true;

    public override byte[] GetData(string url)
    {
        var path = GetPath(url);
        if (File.Exists(path))
        {
            Console.WriteLine($"[ReportStorage] load '{url}' from {path}");
            return File.ReadAllBytes(path);
        }

        // No saved .repx yet — generate from the coded report and persist
        // immediately so the file exists on disk after the very first time
        // the designer is opened. Subsequent saves (and the View / KioskView
        // print paths, which also check File.Exists) then read the same file.
        XtraReport? report = url switch
        {
            "TicketReport" => new TicketReport(),
            "KioskTicketReport" => new KioskTicketReport(),
            _ => null
        };
        if (report == null) throw new FileNotFoundException($"Report '{url}' not found.");

        report.SaveLayoutToXml(path);
        Console.WriteLine($"[ReportStorage] seed '{url}' to {path}");
        return File.ReadAllBytes(path);
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
