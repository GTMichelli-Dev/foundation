using Foundation.Web.Data;
using Foundation.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Web.Services;

public enum PrintSource { Kiosk, Office }

/// <summary>
/// Whether a ticket prints by itself after a weighment (Setup → Printing).
/// Checked in this order, first "no" wins:
///   1. the kiosk's inbound / outbound switches (kiosk only),
///   2. card weigh-ins skip the inbound ticket per Inbound Ticket for Card
///      Weigh-Ins — by default only cards scanned at a kiosk's reader,
///   3. the print rules, first match by sort order,
///   4. When No Print Rule Matches.
/// Reprints don't come through here — someone asked for that ticket.
/// Call after the ticket (and its custom values) has been saved.
/// </summary>
public static class PrintRules
{
    public readonly record struct Decision(bool Print, string Reason);

    /// <param name="cardFromReader">The card was scanned at a kiosk's RFID
    /// reader, as opposed to its number being keyed in.</param>
    public static Decision Decide(ScaleDbContext db, AppSetup setup, Transaction t,
        PrintSource source, bool outbound, bool cardUsed, bool cardFromReader = false)
    {
        if (source == PrintSource.Kiosk)
        {
            if (!outbound && !setup.KioskPrintInbound)
                return new(false, "inbound tickets are turned off at the kiosk");
            if (outbound && !setup.KioskPrintOutbound)
                return new(false, "outbound tickets are turned off at the kiosk");
        }

        if (cardUsed && !outbound && SkipsCardInbound(setup, cardFromReader))
            return new(false, cardFromReader
                ? "RFID card scans don't print an inbound ticket"
                : "card weigh-ins don't print an inbound ticket");

        var rules = db.PrintRules.AsNoTracking()
            .Where(r => r.Active)
            .OrderBy(r => r.SortOrder).ThenBy(r => r.Id)
            .ToList();

        Dictionary<int, string>? custom = null;
        foreach (var r in rules)
        {
            if (!Matches(r, t, source, outbound)) continue;
            if (r.CustomFieldId is int fieldId)
            {
                custom ??= db.TransactionCustomValues.AsNoTracking()
                    .Where(v => v.Ticket == t.Ticket && v.Value != null)
                    .ToDictionary(v => v.CustomFieldId, v => v.Value!);
                if (!Same(r.CustomFieldValue, custom.GetValueOrDefault(fieldId))) continue;
            }
            return new(r.Print, $"print rule \"{r.Name}\"");
        }

        return setup.PrintWhenNoRuleMatches
            ? new(true, "")
            : new(false, "no print rule matched");
    }

    private static bool SkipsCardInbound(AppSetup setup, bool cardFromReader) => setup.CardInboundPrint switch
    {
        AppSetup.CardPrintAlways => false,
        AppSetup.CardPrintSkipAll => true,
        _ => cardFromReader
    };

    private static bool Matches(PrintRule r, Transaction t, PrintSource source, bool outbound)
    {
        if (r.Source != PrintRule.Any
            && !r.Source.Equals(source == PrintSource.Kiosk ? PrintRule.SourceKiosk : PrintRule.SourceOffice, StringComparison.OrdinalIgnoreCase))
            return false;
        if (r.Leg != PrintRule.Any
            && !r.Leg.Equals(outbound ? PrintRule.LegOutbound : PrintRule.LegInbound, StringComparison.OrdinalIgnoreCase))
            return false;

        return Condition(r.Customer, t.Customer)
            && Condition(r.Carrier, t.Carrier)
            && Condition(r.TruckId, t.TruckId)
            && Condition(r.Commodity, t.Commodity)
            && Condition(r.Location, t.Location)
            && Condition(r.Destination, t.Destination);
    }

    /// <summary>A blank condition matches anything; otherwise the values must
    /// agree, ignoring case and surrounding spaces.</summary>
    private static bool Condition(string? wanted, string? actual) =>
        string.IsNullOrWhiteSpace(wanted) || Same(wanted, actual);

    private static bool Same(string? a, string? b) =>
        string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
}
