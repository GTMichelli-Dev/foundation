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
