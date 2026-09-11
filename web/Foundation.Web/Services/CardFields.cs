using Foundation.Web.Data;
using Foundation.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Web.Services;

/// <summary>
/// The set of ticket fields a prox card can carry, and the rules for moving
/// values between a card, the kiosk prompt flow, and a ticket.
///
/// One list serves three callers so they can never drift apart:
///   • the loader operator's Card Setup page renders an input per descriptor,
///   • the kiosk decides which prompts it still has to ask (a field that is
///     required but absent from the card),
///   • weigh-in copies the card's values onto the new ticket.
/// </summary>
public static class CardFields
{
    /// <summary>Card keys that map to standard ticket columns, in the order
    /// the weigh forms use. "truck" is Transaction.TruckId.</summary>
    public static readonly string[] StandardKeys =
        { "commodity", "customer", "carrier", "truck", "location", "destination", "bin", "notes" };

    /// <summary>
    /// One renderable/promptable field. <paramref name="Required"/> means the
    /// kiosk refuses to let the driver past it — that is the only case where a
    /// value missing from the card forces a prompt.
    /// </summary>
    public class Descriptor
    {
        public string Key { get; set; } = "";
        public string Label { get; set; } = "";
        /// <summary>"list", "text", "number", or "truck" (list filtered by the chosen carrier).</summary>
        public string Type { get; set; } = "list";
        public bool Required { get; set; }
        public int Order { get; set; }
        public List<string> Items { get; set; } = new();

        // Custom-field extras
        public int? CfId { get; set; }
        public bool Integer { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public int? Precision { get; set; }
        /// <summary>Cascading sub-field: card key whose answer filters this list.</summary>
        public string? ParentKey { get; set; }
        public Dictionary<string, List<string>>? ValueMap { get; set; }
    }

    /// <summary>
    /// Every field a card can carry, in weigh-form order. Standard fields
    /// hidden in Setup are left out entirely — a card can't set a field the
    /// site doesn't use. Values come from the same master-data tables the
    /// kiosk uses, so a card can only be given a choice a driver could pick.
    /// </summary>
    public static List<Descriptor> Describe(ScaleDbContext db, AppSetup setup, int? siteId = null)
    {
        var customFields = db.CustomFields.Where(f => f.Active).ToList();
        var slots = FieldOrdering.GetFormSlots(setup, customFields);

        var commodities = db.Commodities.Where(c => c.Active && c.UseAtKiosk).ForSite(siteId)
            .OrderBy(c => c.CommodityName).Select(c => c.CommodityName).ToList();
        var customers = db.Customers.Where(c => c.Active && c.UseAtKiosk)
            .OrderBy(c => c.CustomerName).Select(c => c.CustomerName).ToList();
        var carriers = db.Carriers.Where(c => c.Active && c.UseAtKiosk)
            .OrderBy(c => c.CarrierName).Select(c => c.CarrierName).ToList();
        var locations = db.Locations.Where(l => l.Active && l.UseAtKiosk)
            .OrderBy(l => l.LocationName).Select(l => l.LocationName).ToList();
        var destinations = db.Destinations.Where(d => d.Active && d.UseAtKiosk)
            .OrderBy(d => d.DestinationName).Select(d => d.DestinationName).ToList();
        var bins = setup.UseBinInventory
            ? db.Bins.Where(b => b.Active && b.UseAtKiosk).ForSite(siteId)
                .OrderBy(b => b.BinName).Select(b => b.BinName).ToList()
            : new List<string>();

        // Per-parent choice maps for cascading sub-fields, same shape the kiosk
        // receives from /api/kiosk/lists.
        var dependentIds = customFields.Where(f => f.ParentField != null).Select(f => f.Id).ToList();
        var valueMaps = db.CustomFieldListValues
            .Where(v => dependentIds.Contains(v.CustomFieldId))
            .OrderBy(v => v.SortOrder).ThenBy(v => v.Value)
            .AsEnumerable()
            .GroupBy(v => v.CustomFieldId)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(v => v.ParentValue)
                      .ToDictionary(pg => pg.Key, pg => pg.Select(v => v.Value).ToList()));

        var result = new List<Descriptor>();

        foreach (var slot in slots)
        {
            if (slot.Field == null)
            {
                var d = StandardDescriptor(slot.Key, setup);
                if (d == null) continue;
                d.Order = slot.Order;
                d.Items = slot.Key switch
                {
                    "Commodity" => commodities,
                    "Customer" => customers,
                    "Carrier" => carriers,
                    "Location" => locations,
                    "Destination" => destinations,
                    "Bin" => bins,
                    _ => new List<string>()
                };
                result.Add(d);
                continue;
            }

            var f = slot.Field;
            var isNumber = f.FieldType is "Integer" or "Real";
            result.Add(new Descriptor
            {
                Key = "cf" + f.Id,
                Label = f.Name,
                Type = isNumber ? "number" : (f.GetListValues().Count > 0 || f.ParentField != null ? "list" : "text"),
                Required = f.Required,
                Order = slot.Order,
                Items = f.GetListValues(),
                CfId = f.Id,
                Integer = f.FieldType == "Integer",
                MinValue = f.MinValue,
                MaxValue = f.MaxValue,
                Precision = f.Precision,
                ParentKey = CascadeParentKey(f.ParentField),
                ValueMap = f.ParentField != null
                    ? valueMaps.GetValueOrDefault(f.Id) ?? new Dictionary<string, List<string>>()
                    : null
            });
        }

        return result;
    }

    /// <summary>
    /// Descriptor for a standard field slot. Required mirrors the kiosk rule
    /// exactly: the prompt is on for the inbound leg and Skip is not allowed,
    /// so a blank would be refused. Notes and Truck ID get special handling —
    /// Notes is never prompted at a kiosk, and Truck ID's list depends on the
    /// carrier chosen alongside it.
    /// </summary>
    private static Descriptor? StandardDescriptor(string slotKey, AppSetup setup) => slotKey switch
    {
        "Commodity" => new Descriptor
        {
            Key = "commodity", Label = "Commodity",
            Required = setup.PromptKioskCommodityOnInbound && !setup.AllowSkipCommodity
        },
        "Customer" => new Descriptor
        {
            Key = "customer", Label = "Customer",
            Required = setup.PromptKioskCustomerOnInbound && !setup.AllowSkipCustomer
        },
        "Carrier" => new Descriptor
        {
            Key = "carrier", Label = "Carrier",
            Required = setup.PromptKioskCarrier && !setup.AllowSkipCarrier
        },
        "TruckId" => new Descriptor
        {
            Key = "truck", Label = "Truck ID", Type = "truck",
            Required = setup.PromptKioskCarrier && setup.PromptKioskTruckId && !setup.AllowSkipTruckId
        },
        "Location" => new Descriptor
        {
            Key = "location", Label = "Location",
            Required = setup.PromptKioskLocationOnInbound && !setup.AllowSkipLocation
        },
        "Destination" => new Descriptor
        {
            Key = "destination", Label = "Destination",
            Required = setup.PromptKioskDestinationOnInbound && !setup.AllowSkipDestination
        },
        "Bin" => new Descriptor
        {
            Key = "bin", Label = "Bin",
            Required = setup.PromptKioskBinOnInbound && !setup.AllowSkipBin
        },
        // Notes is free text: it rides along on the card but the kiosk never
        // asks for it (no keyboard), so it can never be "required".
        "Notes" => new Descriptor { Key = "notes", Label = "Notes", Type = "text", Required = false },
        _ => null
    };

    /// <summary>CustomField.ParentField ("Commodity", "cf_5") to a card key.</summary>
    public static string? CascadeParentKey(string? parentField)
    {
        if (string.IsNullOrEmpty(parentField)) return null;
        if (parentField.StartsWith("cf_")) return "cf" + parentField[3..];
        // "TruckId" is stored as "truck" on cards and in the kiosk flow.
        if (parentField.Equals("TruckId", StringComparison.OrdinalIgnoreCase)) return "truck";
        return parentField.ToLowerInvariant();
    }

    /// <summary>
    /// Everything stored on a card as { key: value }, standard columns plus
    /// custom values. Blank entries are dropped so callers can treat "present"
    /// as "the card answers this field".
    /// </summary>
    public static Dictionary<string, string> ValuesOf(ScaleDbContext db, Card card)
    {
        var values = new Dictionary<string, string>();

        void Add(string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value)) values[key] = value.Trim();
        }

        Add("commodity", card.Commodity);
        Add("customer", card.Customer);
        Add("carrier", card.Carrier);
        Add("truck", card.TruckId);
        Add("location", card.Location);
        Add("destination", card.Destination);
        Add("bin", card.Bin);
        Add("notes", card.Notes);

        foreach (var v in db.CardCustomValues.Where(v => v.CardId == card.Id).ToList())
            Add("cf" + v.CustomFieldId, v.Value);

        return values;
    }

    /// <summary>
    /// Write a { key: value } set onto a card, standard columns and custom
    /// values alike. Unknown keys are ignored; a blank value clears the field.
    /// Values are validated against the descriptors so a hand-crafted POST
    /// can't put a choice on a card that the weigh forms would reject.
    /// Caller SaveChanges().
    /// </summary>
    public static void ApplyValues(ScaleDbContext db, Card card,
        IReadOnlyDictionary<string, string?> values, List<Descriptor> descriptors)
    {
        foreach (var d in descriptors)
        {
            if (!values.TryGetValue(d.Key, out var raw)) continue;
            var value = raw?.Trim();
            if (value?.Length == 0) value = null;

            if (value != null && !IsValid(d, value, values)) continue;
            if (value != null && value.Length > 200) value = value[..200];

            if (d.CfId is int cfId)
            {
                var existing = db.CardCustomValues
                    .FirstOrDefault(v => v.CardId == card.Id && v.CustomFieldId == cfId);
                if (value == null)
                {
                    if (existing != null) db.CardCustomValues.Remove(existing);
                }
                else if (existing != null)
                {
                    existing.Value = value;
                }
                else
                {
                    db.CardCustomValues.Add(new CardCustomValue
                    {
                        CardId = card.Id, CustomFieldId = cfId, Value = value
                    });
                }
                continue;
            }

            switch (d.Key)
            {
                case "commodity": card.Commodity = Cap(value, 50); break;
                case "customer": card.Customer = Cap(value, 50); break;
                case "carrier": card.Carrier = Cap(value, 50); break;
                case "truck": card.TruckId = Cap(value, 50); break;
                case "location": card.Location = Cap(value, 50); break;
                case "destination": card.Destination = Cap(value, 50); break;
                case "bin": card.Bin = Cap(value, 50); break;
                case "notes": card.Notes = Cap(value, 500); break;
            }
        }
    }

    private static string? Cap(string? value, int max) =>
        value == null ? null : (value.Length > max ? value[..max] : value);

    /// <summary>
    /// Whether a value is one the weigh forms would accept for this field:
    /// a member of the choice list (respecting a cascading parent's answer),
    /// or a number inside the field's min/max/precision.
    /// </summary>
    private static bool IsValid(Descriptor d, string value, IReadOnlyDictionary<string, string?> siblings)
    {
        switch (d.Type)
        {
            case "number":
                if (d.Integer)
                {
                    if (!long.TryParse(value, out var i)) return false;
                    if (d.MinValue.HasValue && i < d.MinValue.Value) return false;
                    if (d.MaxValue.HasValue && i > d.MaxValue.Value) return false;
                    return true;
                }
                if (!double.TryParse(value, out var dbl)) return false;
                if (d.MinValue.HasValue && dbl < d.MinValue.Value) return false;
                if (d.MaxValue.HasValue && dbl > d.MaxValue.Value) return false;
                if (d.Precision.HasValue)
                {
                    var dot = value.IndexOf('.');
                    if (dot >= 0 && value.Length - dot - 1 > d.Precision.Value) return false;
                }
                return true;

            case "text":
                return true;

            case "truck":
                // Trucks are validated against the carrier by the caller (it
                // needs the DB); anything non-empty passes here.
                return true;

            default: // list
                if (d.ValueMap != null)
                {
                    // Cascading sub-field: valid choices depend on the parent
                    // answer being saved in the same request. No parent answer
                    // means any mapped choice is acceptable.
                    var parent = d.ParentKey != null ? siblings.GetValueOrDefault(d.ParentKey) : null;
                    if (!string.IsNullOrWhiteSpace(parent))
                        return d.ValueMap.GetValueOrDefault(parent.Trim())?.Contains(value) == true;
                    return d.ValueMap.Values.Any(list => list.Contains(value));
                }
                return d.Items.Count == 0 || d.Items.Contains(value);
        }
    }

    /// <summary>
    /// Release a card once its transaction closes. Recycling keeps the card
    /// issued with its stored values so the driver can run another load;
    /// otherwise it is deactivated until the loader operator re-issues it.
    /// Either way the open-ticket link is cleared. Caller SaveChanges().
    /// </summary>
    public static void Release(Card card, AppSetup setup, string ticket)
    {
        card.OpenTicket = null;
        card.LastTicket = ticket;
        card.LastUsedAt = DateTime.UtcNow;

        if (!card.RecyclesUnder(setup))
        {
            card.Issued = false;
            card.IssuedAt = null;
        }
    }

    /// <summary>The enrolled card with this number (trimmed, any case), or null.</summary>
    public static Card? Find(ScaleDbContext db, string? number)
    {
        var n = (number ?? "").Trim();
        if (n.Length == 0) return null;
        return db.Cards.AsEnumerable()
            .FirstOrDefault(c => string.Equals(c.CardNumber, n, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>What a card means right now, wherever it was used.</summary>
    public class Resolution
    {
        public bool Ok { get; init; }
        /// <summary>Why not, for callers that branch on it: "empty", "unknown",
        /// "disabled-card" or "not-issued".</summary>
        public string? Reason { get; init; }
        public string? Message { get; init; }
        /// <summary>"weighin", or "weighout" when the card's load is already open.</summary>
        public string? Action { get; init; }
        public Card? Card { get; init; }
        public Dictionary<string, string> Values { get; init; } = new();
        /// <summary>The open ticket a weigh-out resolves to.</summary>
        public Transaction? Ticket { get; init; }
        /// <summary>Required fields the card leaves blank, which must be asked.</summary>
        public List<string> MissingRequired { get; init; } = new();
        public int? RetainedTare { get; init; }

        public object? Summary => Card == null ? null
            : new { id = Card.Id, cardNumber = Card.CardNumber, description = Card.Description };
    }

    /// <summary>A translate function with a translator's indexer (key in, text out).</summary>
    public readonly struct Texts
    {
        private readonly Func<string, string> _translate;
        public Texts(Func<string, string> translate) => _translate = translate;
        public string this[string key] => _translate(key);
    }

    /// <summary>
    /// Resolve a card someone presented or keyed in: a weigh-in with the
    /// card's stored values, or a weigh-out of the load it is already on. The
    /// kiosk, the phone and the weigh forms all come through here so a card
    /// means the same thing wherever it is used. <paramref name="translate"/>
    /// puts the refusal messages in the language of the screen asking.
    /// </summary>
    public static Resolution Resolve(ScaleDbContext db, AppSetup setup, string? cardNumber, int? scaleId, Func<string, string> translate)
    {
        // Named and indexed like a controller's translator so that
        // scripts/check-translations.py finds the keys below and checks them.
        var _t = new Texts(translate);

        if (string.IsNullOrWhiteSpace(cardNumber))
            return new Resolution { Reason = "empty", Message = _t["No card number"] };

        var card = Find(db, cardNumber);
        if (card == null)
            return new Resolution { Reason = "unknown", Message = _t["Card Not Recognized"] };
        if (!card.Enabled)
            return new Resolution { Reason = "disabled-card", Message = _t["Card Disabled"] };

        // An open ticket outranks the issued flag: a driver who weighed in must
        // always be able to weigh out, even if the card was deactivated behind
        // them. Voided/closed tickets fall through to the weigh-in path.
        Transaction? openTicket = null;
        if (!string.IsNullOrEmpty(card.OpenTicket))
        {
            openTicket = db.Transactions
                .FirstOrDefault(x => x.Ticket == card.OpenTicket && !x.Void && x.DateOut == null);
            if (openTicket == null)
            {
                card.OpenTicket = null; // stale link
                db.SaveChanges();
            }
        }

        if (openTicket == null && !card.Issued)
            return new Resolution { Reason = "not-issued", Message = _t["Card Not Active — See Loader Operator"] };

        var values = ValuesOf(db, card);

        if (openTicket != null)
            return new Resolution { Ok = true, Action = "weighout", Card = card, Values = values, Ticket = openTicket };

        // No open ticket on the card, but the truck it names might already be
        // in the yard from a keyed-in weigh-in. Adopt that ticket rather than
        // opening a second one for the same truck.
        if (values.TryGetValue("carrier", out var cardCarrier)
            && values.TryGetValue("truck", out var cardTruck))
        {
            var truckTicket = db.Transactions
                .Where(x => !x.Void && x.DateOut == null && x.Carrier == cardCarrier && x.TruckId == cardTruck)
                .OrderByDescending(x => x.DateIn)
                .FirstOrDefault();
            if (truckTicket != null)
                return new Resolution { Ok = true, Action = "weighout", Card = card, Values = values, Ticket = truckTicket };
        }

        var siteId = scaleId.HasValue ? db.Scales.Find(scaleId.Value)?.SiteId : null;
        var missing = Describe(db, setup, siteId)
            .Where(d => d.Required && !values.ContainsKey(d.Key))
            .Select(d => d.Key)
            .ToList();

        // Retained tare: reported so the screen can tell the driver the load
        // will finish in one weighment. The weigh-in does the actual completing.
        int? retainedTare = null;
        if (setup.UseRetainedTare
            && values.TryGetValue("carrier", out var c2)
            && values.TryGetValue("truck", out var t2))
        {
            var truck = db.Trucks.FirstOrDefault(x => x.TruckId == t2 && x.CarrierName == c2);
            if (truck?.RetainedTare.HasValue == true
                && (!setup.AutoClearStaleRetainedTare
                    || (truck.RetainedTareUpdated?.Date ?? DateTime.MinValue) >= DateTime.Today))
            {
                retainedTare = truck.RetainedTare;
            }
        }

        return new Resolution
        {
            Ok = true, Action = "weighin", Card = card, Values = values,
            MissingRequired = missing, RetainedTare = retainedTare
        };
    }

    /// <summary>
    /// Copy the card's custom-field values onto a new ticket, skipping any
    /// field the driver just answered. Card values were validated when the
    /// card was issued, and they cover fields a kiosk or phone can't prompt
    /// for at all (free text), which is much of the point of the card.
    /// Caller SaveChanges().
    /// </summary>
    public static void CopyCustomValues(ScaleDbContext db, string ticket, Card card, HashSet<int> alreadyWritten)
    {
        foreach (var v in db.CardCustomValues.Where(v => v.CardId == card.Id).ToList())
        {
            if (alreadyWritten.Contains(v.CustomFieldId)) continue;
            if (string.IsNullOrWhiteSpace(v.Value)) continue;
            db.TransactionCustomValues.Add(new TransactionCustomValue
            {
                Ticket = ticket,
                CustomFieldId = v.CustomFieldId,
                Value = v.Value
            });
        }
    }

    /// <summary>
    /// Tie a card to the ticket it just weighed in on, so presenting it again
    /// weighs that load out — or release it outright when the ticket closed in
    /// the same weighment (retained tare). Returns whether a released card
    /// recycles. Caller SaveChanges().
    /// </summary>
    public static bool Bind(Card card, AppSetup setup, string ticket, bool closed)
    {
        if (closed)
        {
            var recycles = card.RecyclesUnder(setup);
            Release(card, setup, ticket);
            return recycles;
        }
        card.OpenTicket = ticket;
        card.LastUsedAt = DateTime.UtcNow;
        return false;
    }

    /// <summary>
    /// The card bound to a ticket, or null. Looks through Card.OpenTicket
    /// first (the live link) and falls back to the card number stamped on the
    /// transaction, so a ticket closed from the office still frees its card.
    /// </summary>
    public static Card? ForTicket(ScaleDbContext db, Transaction tx)
    {
        var byOpen = db.Cards.FirstOrDefault(c => c.OpenTicket == tx.Ticket);
        if (byOpen != null) return byOpen;
        return string.IsNullOrEmpty(tx.CardNumber)
            ? null
            : db.Cards.FirstOrDefault(c => c.CardNumber == tx.CardNumber);
    }
}
