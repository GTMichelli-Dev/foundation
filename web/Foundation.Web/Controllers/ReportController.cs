using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Foundation.Web.Data;
using Foundation.Web.Models;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

public class ReportController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly AppSetupCache _setupCache;

    public ReportController(ScaleDbContext db, AppSetupCache setupCache)
    {
        _db = db;
        _setupCache = setupCache;
    }

    public IActionResult Index()
    {
        var setup = _setupCache.Get();
        ViewBag.CompanyName = setup.Header1 ?? "Foundation";
        ViewBag.SavePicture = setup.SavePicture;
        ViewBag.UseQuickBooks = setup.UseQuickBooks;
        ViewBag.UseBinInventory = setup.UseBinInventory;
        return View();
    }

    [HttpGet("api/reports/transactions")]
    public IActionResult GetTransactions(DateTime? startDate, DateTime? endDate)
    {
        // Filter range in the configured display TZ. endInclusive = midnight
        // at the START of (endDate + 1) so the chosen endDate includes every
        // record through 23:59:59.999 of that local date.
        var localStart = startDate ?? DateTime.Today.AddDays(-30);
        var localEnd   = (endDate ?? DateTime.Today).Date.AddDays(1);
        var start = AppTimeZone.ToUtc(localStart);
        var end   = AppTimeZone.ToUtc(localEnd);

        // Count voided tickets in the date range
        var voidCount = _db.Transactions
            .Count(t => t.DateOut != null && t.Void && t.DateOut >= start && t.DateOut < end);

        // Only completed (has DateOut), non-voided trucks, filtered by DateOut
        var rows = _db.Transactions
            .Where(t => t.DateOut != null && !t.Void && t.DateOut >= start && t.DateOut < end)
            .OrderByDescending(t => t.DateOut)
            .ToList();

        var ticketIds = rows.Select(t => t.Ticket).ToList();
        var customValues = _db.TransactionCustomValues
            .Where(v => ticketIds.Contains(v.Ticket))
            .AsEnumerable()
            .GroupBy(v => v.Ticket)
            .ToDictionary(g => g.Key,
                g => g.ToDictionary(v => v.CustomFieldId.ToString(), v => v.Value));

        var results = rows
            .Select(t => new
            {
                t.Ticket,
                DateIn = t.DateIn.AsUtc(),
                DateOut = t.DateOut.AsUtc(),
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.Location,
                t.Destination,
                t.Bin,
                t.InWeight,
                t.OutWeight,
                t.NetWeight,
                NetTons = Math.Round(t.NetWeight / 2000.0, 2),
                t.Notes,
                t.SentToQuickBooks,
                HasInImage = System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tickets", $"{t.Ticket}_in.jpg")),
                HasOutImage = System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tickets", $"{t.Ticket}_out.jpg")),
                CustomFields = customValues.GetValueOrDefault(t.Ticket)
            })
            .ToList();

        return Json(new { data = results, voidCount });
    }

    // ===== REPORT TEMPLATES (report builder) =====
    // Named layouts of the Transactions grid: columns/order/grouping/sort/
    // filters saved as the DevExtreme grid state. Server-side so every
    // workstation sees the same saved reports.

    [HttpGet("api/reports/templates")]
    public IActionResult GetTemplates() =>
        Json(_db.ReportTemplates.OrderBy(t => t.Name)
            .Select(t => new { t.Id, t.Name, Updated = ((DateTime?)t.Updated).AsUtc() })
            .ToList());

    [HttpGet("api/reports/templates/{id:int}")]
    public IActionResult GetTemplate(int id)
    {
        var t = _db.ReportTemplates.Find(id);
        if (t == null) return NotFound();
        return Json(new { t.Id, t.Name, t.StateJson });
    }

    public class ReportTemplateRequest
    {
        public string Name { get; set; } = "";
        public string StateJson { get; set; } = "{}";
    }

    [HttpPost("api/reports/templates")]
    public IActionResult AddTemplate([FromBody] ReportTemplateRequest request)
    {
        var error = ValidateTemplate(request, null);
        if (error != null) return BadRequest(new { message = error });

        var template = new ReportTemplate
        {
            Name = request.Name.Trim(),
            StateJson = request.StateJson,
            Updated = DateTime.UtcNow
        };
        _db.ReportTemplates.Add(template);
        _db.SaveChanges();
        return Json(new { template.Id, template.Name });
    }

    [HttpPut("api/reports/templates/{id:int}")]
    public IActionResult UpdateTemplate(int id, [FromBody] ReportTemplateRequest request)
    {
        var existing = _db.ReportTemplates.Find(id);
        if (existing == null) return NotFound();

        var error = ValidateTemplate(request, id);
        if (error != null) return BadRequest(new { message = error });

        existing.Name = request.Name.Trim();
        existing.StateJson = request.StateJson;
        existing.Updated = DateTime.UtcNow;
        _db.SaveChanges();
        return Json(new { existing.Id, existing.Name });
    }

    /// <summary>
    /// Run a saved report template server-side: applies the template's column
    /// selection, per-column filters, and sorting to the completed tickets in
    /// the date range (defaults: last 30 days) and returns flat JSON rows with
    /// only the template's columns. Grouping is returned as metadata (groupBy)
    /// since JSON rows are flat. Complex filter-panel expressions saved in the
    /// layout are not evaluated — only per-column filter row / header filters.
    /// Custom-field columns come back under their cf_{id} keys.
    /// </summary>
    [HttpGet("api/reports/templates/{id:int}/records")]
    public IActionResult GetTemplateRecords(int id, DateTime? startDate, DateTime? endDate)
    {
        var template = _db.ReportTemplates.Find(id);
        if (template == null) return NotFound(new { message = "Report template not found." });

        var localStart = startDate ?? DateTime.Today.AddDays(-30);
        var localEnd = (endDate ?? DateTime.Today).Date.AddDays(1);
        var start = AppTimeZone.ToUtc(localStart);
        var end = AppTimeZone.ToUtc(localEnd);

        var rows = _db.Transactions
            .Where(t => t.DateOut != null && !t.Void && t.DateOut >= start && t.DateOut < end)
            .OrderByDescending(t => t.DateOut)
            .ToList();

        var ticketIds = rows.Select(t => t.Ticket).ToList();
        var customValues = _db.TransactionCustomValues
            .Where(v => ticketIds.Contains(v.Ticket))
            .AsEnumerable()
            .GroupBy(v => v.Ticket)
            .ToDictionary(g => g.Key,
                g => g.ToDictionary(v => "cf_" + v.CustomFieldId, v => (object?)v.Value));

        var data = rows.Select(t =>
        {
            var d = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["ticket"] = t.Ticket,
                ["dateIn"] = t.DateIn.AsUtc(),
                ["dateOut"] = t.DateOut.AsUtc(),
                ["customer"] = t.Customer,
                ["carrier"] = t.Carrier,
                ["truckId"] = t.TruckId,
                ["commodity"] = t.Commodity,
                ["location"] = t.Location,
                ["destination"] = t.Destination,
                ["bin"] = t.Bin,
                ["inWeight"] = t.InWeight,
                ["outWeight"] = t.OutWeight,
                ["netWeight"] = t.NetWeight,
                ["netTons"] = Math.Round(t.NetWeight / 2000.0, 2),
                ["notes"] = t.Notes,
                ["sentToQuickBooks"] = t.SentToQuickBooks
            };
            if (customValues.TryGetValue(t.Ticket, out var cvs))
                foreach (var kv in cvs) d[kv.Key] = kv.Value;
            return d;
        }).ToList();

        var columns = ParseTemplateColumns(template.StateJson);

        // Per-column filters from the saved layout
        foreach (var col in columns)
        {
            if (col.FilterValues is { Count: > 0 })
            {
                var wanted = col.FilterValues
                    .Where(v => v.ValueKind is JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
                    .Select(v => ValueText(JsonToValue(v)))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (wanted.Count > 0)
                    data = data.Where(r => wanted.Contains(ValueText(r.GetValueOrDefault(col.Key)))).ToList();
            }
            else if (col.FilterValue.HasValue && col.FilterValue.Value.ValueKind != JsonValueKind.Null)
            {
                data = data.Where(r => FilterMatch(r.GetValueOrDefault(col.Key), col.Op, col.FilterValue.Value)).ToList();
            }
        }

        // Sorting from the saved layout (group columns sort first, then sortIndex)
        var sortCols = columns
            .Where(c => c.GroupIndex.HasValue || (c.SortIndex.HasValue && c.SortOrder != null))
            .OrderBy(c => c.GroupIndex.HasValue ? 0 : 1)
            .ThenBy(c => c.GroupIndex ?? c.SortIndex)
            .ToList();
        if (sortCols.Count > 0)
        {
            IOrderedEnumerable<Dictionary<string, object?>>? ordered = null;
            foreach (var col in sortCols)
            {
                var desc = col.SortOrder == "desc";
                Func<Dictionary<string, object?>, object?> sel = r => r.GetValueOrDefault(col.Key);
                ordered = ordered == null
                    ? (desc ? data.OrderByDescending(sel, ValueComparer.Instance) : data.OrderBy(sel, ValueComparer.Instance))
                    : (desc ? ordered.ThenByDescending(sel, ValueComparer.Instance) : ordered.ThenBy(sel, ValueComparer.Instance));
            }
            data = ordered!.ToList();
        }

        // Output columns: group columns first, then visible columns in display order
        var groupBy = columns.Where(c => c.GroupIndex.HasValue)
            .OrderBy(c => c.GroupIndex).Select(c => c.Key).ToList();
        var visible = columns.Where(c => c.Visible && !c.GroupIndex.HasValue)
            .OrderBy(c => c.VisibleIndex ?? int.MaxValue).Select(c => c.Key).ToList();
        var outputColumns = groupBy.Concat(visible).ToList();
        if (outputColumns.Count == 0) outputColumns = data.FirstOrDefault()?.Keys.ToList() ?? new List<string>();

        var shaped = data.Select(r =>
        {
            var o = new Dictionary<string, object?>();
            foreach (var c in outputColumns) o[c] = r.GetValueOrDefault(c);
            return o;
        }).ToList();

        return Json(new
        {
            id = template.Id,
            name = template.Name,
            startDate = localStart.ToString("yyyy-MM-dd"),
            endDate = (localEnd.AddDays(-1)).ToString("yyyy-MM-dd"),
            groupBy,
            columns = outputColumns,
            count = shaped.Count,
            data = shaped
        });
    }

    private sealed record TemplateColumn(string Key, bool Visible, int? VisibleIndex, int? SortIndex,
        string? SortOrder, int? GroupIndex, JsonElement? FilterValue, List<JsonElement>? FilterValues, string? Op);

    private static List<TemplateColumn> ParseTemplateColumns(string stateJson)
    {
        var result = new List<TemplateColumn>();
        try
        {
            using var doc = JsonDocument.Parse(stateJson);
            if (!doc.RootElement.TryGetProperty("columns", out var cols) || cols.ValueKind != JsonValueKind.Array)
                return result;
            foreach (var c in cols.EnumerateArray())
            {
                var key = c.TryGetProperty("dataField", out var df) && df.ValueKind == JsonValueKind.String
                    ? df.GetString()
                    : c.TryGetProperty("name", out var nm) && nm.ValueKind == JsonValueKind.String ? nm.GetString() : null;
                if (string.IsNullOrEmpty(key)) continue;

                int? IntOf(string prop) => c.TryGetProperty(prop, out var e) && e.ValueKind == JsonValueKind.Number ? e.GetInt32() : null;
                string? StrOf(string prop) => c.TryGetProperty(prop, out var e) && e.ValueKind == JsonValueKind.String ? e.GetString() : null;

                JsonElement? filterValue = c.TryGetProperty("filterValue", out var fv) && fv.ValueKind != JsonValueKind.Null
                    ? fv.Clone() : null;
                List<JsonElement>? filterValues = c.TryGetProperty("filterValues", out var fvs) && fvs.ValueKind == JsonValueKind.Array
                    ? fvs.EnumerateArray().Select(e => e.Clone()).ToList() : null;

                result.Add(new TemplateColumn(
                    key,
                    !c.TryGetProperty("visible", out var vis) || vis.ValueKind != JsonValueKind.False,
                    IntOf("visibleIndex"), IntOf("sortIndex"), StrOf("sortOrder"), IntOf("groupIndex"),
                    filterValue, filterValues, StrOf("selectedFilterOperation")));
            }
        }
        catch (JsonException)
        {
            // Corrupt layout — run the report unfiltered with default columns.
        }
        return result;
    }

    private static object? JsonToValue(JsonElement e) => e.ValueKind switch
    {
        JsonValueKind.String => e.GetString(),
        JsonValueKind.Number => e.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        _ => null
    };

    private static string ValueText(object? v) => v switch
    {
        null => "",
        double d => d.ToString("0.####"),
        int i => i.ToString(),
        long l => l.ToString(),
        DateTime dt => dt.ToString("o"),
        _ => v.ToString() ?? ""
    };

    private static bool TryNumber(object? v, out double n)
    {
        switch (v)
        {
            case int i: n = i; return true;
            case long l: n = l; return true;
            case double d: n = d; return true;
            case string s when double.TryParse(s, out var p): n = p; return true;
            default: n = 0; return false;
        }
    }

    private static bool FilterMatch(object? val, string? op, JsonElement filter)
    {
        // "between" carries a two-element array
        if (filter.ValueKind == JsonValueKind.Array)
        {
            var parts = filter.EnumerateArray().ToList();
            if (parts.Count != 2) return true; // unsupported shape — don't filter
            return FilterMatch(val, ">=", parts[0]) && FilterMatch(val, "<=", parts[1]);
        }

        var fv = JsonToValue(filter);
        var text = ValueText(val);
        var ftext = ValueText(fv);

        // Dates compare chronologically when both sides parse
        if (val is DateTime dt && DateTime.TryParse(ftext, null, System.Globalization.DateTimeStyles.RoundtripKind, out var fdt))
            return CompareOp(dt.CompareTo(fdt), op ?? "=");

        // Numbers compare numerically when both sides parse
        if (TryNumber(val, out var nv) && TryNumber(fv, out var nf) && op is "=" or "<>" or "<" or "<=" or ">" or ">=")
            return CompareOp(nv.CompareTo(nf), op);

        return (op ?? (fv is string ? "contains" : "=")) switch
        {
            "contains" => text.Contains(ftext, StringComparison.OrdinalIgnoreCase),
            "notcontains" => !text.Contains(ftext, StringComparison.OrdinalIgnoreCase),
            "startswith" => text.StartsWith(ftext, StringComparison.OrdinalIgnoreCase),
            "endswith" => text.EndsWith(ftext, StringComparison.OrdinalIgnoreCase),
            "=" => string.Equals(text, ftext, StringComparison.OrdinalIgnoreCase),
            "<>" => !string.Equals(text, ftext, StringComparison.OrdinalIgnoreCase),
            "<" => string.Compare(text, ftext, StringComparison.OrdinalIgnoreCase) < 0,
            "<=" => string.Compare(text, ftext, StringComparison.OrdinalIgnoreCase) <= 0,
            ">" => string.Compare(text, ftext, StringComparison.OrdinalIgnoreCase) > 0,
            ">=" => string.Compare(text, ftext, StringComparison.OrdinalIgnoreCase) >= 0,
            _ => true
        };
    }

    private static bool CompareOp(int cmp, string op) => op switch
    {
        "=" => cmp == 0,
        "<>" => cmp != 0,
        "<" => cmp < 0,
        "<=" => cmp <= 0,
        ">" => cmp > 0,
        ">=" => cmp >= 0,
        _ => cmp == 0
    };

    /// <summary>Orders mixed row values: numbers numerically, dates
    /// chronologically, everything else as case-insensitive text.</summary>
    private sealed class ValueComparer : IComparer<object?>
    {
        public static readonly ValueComparer Instance = new();
        public int Compare(object? x, object? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            if (x is DateTime dx && y is DateTime dy) return dx.CompareTo(dy);
            if (TryNumber(x, out var nx) && TryNumber(y, out var ny)) return nx.CompareTo(ny);
            return string.Compare(ValueText(x), ValueText(y), StringComparison.OrdinalIgnoreCase);
        }
    }

    [HttpDelete("api/reports/templates/{id:int}")]
    public IActionResult DeleteTemplate(int id)
    {
        var existing = _db.ReportTemplates.Find(id);
        if (existing == null) return NotFound();
        _db.ReportTemplates.Remove(existing);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    private string? ValidateTemplate(ReportTemplateRequest request, int? id)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) return "Report name is required.";
        var name = request.Name.Trim();
        if (name.Length > 100) return "Report name must be 100 characters or fewer.";
        if (string.IsNullOrWhiteSpace(request.StateJson)) return "Report layout is missing.";
        if (request.StateJson.Length > 200_000) return "Report layout is too large.";
        if (_db.ReportTemplates.Any(t => t.Id != id && t.Name.ToLower() == name.ToLower()))
            return $"A report named \"{name}\" already exists.";
        return null;
    }

    // ===== BIN INVENTORY (Setup → Options → Use Bin Inventory) =====
    //
    // On-hand per (bin, commodity) is computed from ticket history rather than
    // a stored counter, so it can be reported as of any date:
    //   - truck arrived heavy (InWeight > OutWeight)  → load INTO the bin
    //   - truck left heavy   (OutWeight > InWeight)   → load OUT of the bin
    //   - plus manual BinAdjustments (true-ups for shrinkage, starting balances)

    private sealed class BinInvRow
    {
        public int LoadsIn; public long LbsIn;
        public int LoadsOut; public long LbsOut;
        public long AdjustmentLbs;
        public long OnHand => LbsIn - LbsOut + AdjustmentLbs;
    }

    /// <summary>Accumulate completed non-void binned tickets and adjustments
    /// into per-(bin, commodity) rows. cutoff = exclusive UTC upper bound, or
    /// null for all history.</summary>
    private Dictionary<(string Bin, string Commodity), BinInvRow> ComputeBinInventory(DateTime? cutoff)
    {
        var rows = new Dictionary<(string, string), BinInvRow>();

        BinInvRow Row(string bin, string? commodity)
        {
            var key = (bin, commodity ?? "");
            if (!rows.TryGetValue(key, out var r)) rows[key] = r = new BinInvRow();
            return r;
        }

        var txns = _db.Transactions
            .Where(t => !t.Void && t.DateOut != null && t.OutWeight != null
                        && t.Bin != null && t.Bin != "")
            .Where(t => cutoff == null || t.DateOut < cutoff)
            .Select(t => new { t.Bin, t.Commodity, t.InWeight, t.OutWeight })
            .ToList();

        foreach (var t in txns)
        {
            var net = Math.Abs(t.InWeight - t.OutWeight!.Value);
            if (net == 0) continue;
            var r = Row(t.Bin!, t.Commodity);
            if (t.InWeight > t.OutWeight.Value) { r.LoadsIn++; r.LbsIn += net; }
            else { r.LoadsOut++; r.LbsOut += net; }
        }

        var adjustments = _db.BinAdjustments
            .Where(a => cutoff == null || a.Date < cutoff)
            .ToList();
        foreach (var a in adjustments)
            Row(a.Bin, a.Commodity).AdjustmentLbs += a.AmountLbs;

        return rows;
    }

    [HttpGet("api/reports/bin-inventory")]
    public IActionResult GetBinInventory(DateTime? asOfDate)
    {
        // Inclusive as-of date in the display TZ → exclusive UTC upper bound.
        DateTime? cutoff = asOfDate.HasValue
            ? AppTimeZone.ToUtc(asOfDate.Value.Date.AddDays(1))
            : null;

        var rows = ComputeBinInventory(cutoff);

        // Active bins with no movements still get a zero row so the operator
        // sees every bin on the report.
        foreach (var bin in _db.Bins.Where(b => b.Active).Select(b => b.BinName).ToList())
        {
            if (!rows.Keys.Any(k => k.Bin == bin))
                rows[(bin, "")] = new BinInvRow();
        }

        var results = rows
            .OrderBy(kv => kv.Key.Bin).ThenBy(kv => kv.Key.Commodity)
            .Select(kv => new
            {
                bin = kv.Key.Bin,
                commodity = kv.Key.Commodity == "" ? null : kv.Key.Commodity,
                loadsIn = kv.Value.LoadsIn,
                lbsIn = kv.Value.LbsIn,
                loadsOut = kv.Value.LoadsOut,
                lbsOut = kv.Value.LbsOut,
                adjustmentLbs = kv.Value.AdjustmentLbs,
                onHandLbs = kv.Value.OnHand,
                onHandTons = Math.Round(kv.Value.OnHand / 2000.0, 2)
            })
            .ToList();

        return Json(results);
    }

    /// <summary>Movement history for one (bin, commodity) row — the tickets
    /// and adjustments behind the computed balance, newest first.</summary>
    [HttpGet("api/reports/bin-inventory/detail")]
    public IActionResult GetBinInventoryDetail(string bin, string? commodity, DateTime? asOfDate)
    {
        if (string.IsNullOrWhiteSpace(bin)) return BadRequest(new { message = "bin is required" });
        DateTime? cutoff = asOfDate.HasValue
            ? AppTimeZone.ToUtc(asOfDate.Value.Date.AddDays(1))
            : null;
        commodity = string.IsNullOrEmpty(commodity) ? null : commodity;

        var tickets = _db.Transactions
            .Where(t => !t.Void && t.DateOut != null && t.OutWeight != null && t.Bin == bin)
            .Where(t => commodity == null ? (t.Commodity == null || t.Commodity == "") : t.Commodity == commodity)
            .Where(t => cutoff == null || t.DateOut < cutoff)
            .ToList()
            .Where(t => t.NetWeight != 0)
            .Select(t => new
            {
                type = t.InWeight > t.OutWeight!.Value ? "In" : "Out",
                date = t.DateOut.AsUtc(),
                reference = "Ticket #" + t.Ticket,
                lbs = (t.InWeight > t.OutWeight.Value ? 1 : -1) * (long)t.NetWeight,
                adjustmentId = (int?)null
            });

        var adjustments = _db.BinAdjustments
            .Where(a => a.Bin == bin)
            .Where(a => commodity == null ? (a.Commodity == null || a.Commodity == "") : a.Commodity == commodity)
            .Where(a => cutoff == null || a.Date < cutoff)
            .ToList()
            .Select(a => new
            {
                type = "Adjustment",
                date = ((DateTime?)a.Date).AsUtc(),
                reference = string.IsNullOrEmpty(a.Note) ? "Adjustment" : a.Note,
                lbs = (long)a.AmountLbs,
                adjustmentId = (int?)a.Id
            });

        return Json(tickets.Concat(adjustments).OrderByDescending(m => m.date).ToList());
    }

    public class BinAdjustmentRequest
    {
        public string Bin { get; set; } = string.Empty;
        public string? Commodity { get; set; }
        /// <summary>"adjust" stores AmountLbs as a signed delta; "trueup"
        /// treats AmountLbs as the measured on-hand total and stores the
        /// difference from the computed balance.</summary>
        public string Mode { get; set; } = "adjust";
        public int AmountLbs { get; set; }
        public string? Note { get; set; }
    }

    [HttpPost("api/reports/bin-adjustments")]
    public IActionResult AddBinAdjustment([FromBody] BinAdjustmentRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Bin))
            return BadRequest(new { message = "Bin is required." });

        var commodity = string.IsNullOrWhiteSpace(request.Commodity) ? null : request.Commodity.Trim();
        int delta;
        string? note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();

        if (string.Equals(request.Mode, "trueup", StringComparison.OrdinalIgnoreCase))
        {
            // Measured total → signed delta from the current computed balance.
            var rows = ComputeBinInventory(null);
            rows.TryGetValue((request.Bin, commodity ?? ""), out var row);
            var current = row?.OnHand ?? 0;
            delta = (int)Math.Clamp(request.AmountLbs - current, int.MinValue, int.MaxValue);
            note ??= $"True-up to {request.AmountLbs:#,##0} lb";
            if (delta == 0)
                return Json(new { success = true, amountLbs = 0, message = "Already at the measured amount — no adjustment recorded." });
        }
        else
        {
            delta = request.AmountLbs;
            if (delta == 0) return BadRequest(new { message = "Adjustment amount cannot be zero." });
        }

        var adjustment = new BinAdjustment
        {
            Bin = request.Bin.Trim(),
            Commodity = commodity,
            AmountLbs = delta,
            Date = DateTime.UtcNow,
            Note = note
        };
        _db.BinAdjustments.Add(adjustment);
        _db.SaveChanges();

        return Json(new { success = true, amountLbs = delta, id = adjustment.Id });
    }

    [HttpDelete("api/reports/bin-adjustments/{id:int}")]
    public IActionResult DeleteBinAdjustment(int id)
    {
        var adjustment = _db.BinAdjustments.Find(id);
        if (adjustment == null) return NotFound();
        _db.BinAdjustments.Remove(adjustment);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    [HttpGet("api/reports/voided")]
    public IActionResult GetVoided(DateTime? startDate, DateTime? endDate)
    {
        var localStart = startDate ?? DateTime.Today.AddDays(-30);
        var localEnd   = (endDate ?? DateTime.Today).Date.AddDays(1);
        var start = AppTimeZone.ToUtc(localStart);
        var end   = AppTimeZone.ToUtc(localEnd);

        var results = _db.Transactions
            .Where(t => t.DateOut != null && t.Void && t.DateOut >= start && t.DateOut < end)
            .OrderByDescending(t => t.DateOut)
            .ToList()
            .Select(t => new
            {
                t.Ticket,
                DateIn = t.DateIn.AsUtc(),
                DateOut = t.DateOut.AsUtc(),
                t.Customer,
                t.Carrier,
                t.TruckId,
                t.Commodity,
                t.InWeight,
                t.OutWeight,
                t.NetWeight,
                t.Notes
            })
            .ToList();

        return Json(results);
    }
}
