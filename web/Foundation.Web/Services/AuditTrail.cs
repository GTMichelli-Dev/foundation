using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text;
using Foundation.Web.Controllers;
using Foundation.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Foundation.Web.Services;

/// <summary>
/// The audit trail. ScaleDbContext calls <see cref="Capture"/> before every
/// save and <see cref="Complete"/> after it, so every change made anywhere in
/// the app — weigh forms, kiosk, phone, Tables, Setup, device services — lands
/// in AuditLogs without any controller having to remember to log it.
///
/// Each changed field is a row with its old and new value; a created or
/// deleted record is one row with a summary of its values. The user is
/// recorded when Require Login is on; the source (office, which kiosk, phone,
/// …) always is. Auditing can never fail a save: a problem building the rows
/// is logged to the console and the save goes ahead without them.
/// </summary>
public static class AuditTrail
{
    public sealed class Pending
    {
        public required EntityEntry Entry { get; init; }
        public required AuditLog Row { get; init; }
        /// <summary>The record is new and its key is assigned by the database,
        /// so the key can only be read once the save is done.</summary>
        public bool KeyAfterSave { get; init; }
    }

    private const int MaxValue = 1000;

    /// <summary>Records whose changes are not worth a row: the audit itself,
    /// grid layouts, the load-email de-dup log and report designer layouts.</summary>
    private static readonly HashSet<Type> Skipped = new()
    {
        typeof(AuditLog), typeof(GridLayout), typeof(LoadEmailLog), typeof(ReportTemplate)
    };

    /// <summary>Fields that change on their own, constantly, and mean nothing
    /// to a reader: a kiosk's heartbeat.</summary>
    private static readonly HashSet<(Type, string)> Ignored = new()
    {
        (typeof(Kiosk), nameof(Kiosk.LastSeenAt))
    };

    public static List<Pending> Capture(DbContext db, HttpContext? http)
    {
        try
        {
            return CaptureCore(db, http);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Audit] capture failed, saving without audit rows: {ex.Message}");
            return new List<Pending>();
        }
    }

    /// <summary>Fill in keys the database assigned and queue the rows. The
    /// caller saves them.</summary>
    public static void Complete(DbContext db, List<Pending> pending)
    {
        foreach (var p in pending)
        {
            if (p.KeyAfterSave) p.Row.EntityKey = Clip(KeyOf(p.Entry), 100);
            db.Set<AuditLog>().Add(p.Row);
        }
    }

    private static List<Pending> CaptureCore(DbContext db, HttpContext? http)
    {
        db.ChangeTracker.DetectChanges();
        var entries = db.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                        && !Skipped.Contains(e.Entity.GetType()))
            .ToList();
        var result = new List<Pending>();
        if (entries.Count == 0) return result;

        var path = http?.Request.Path.Value ?? "";
        var (user, source) = Who(db, http, path);
        var now = DateTime.UtcNow;
        Dictionary<int, string>? fieldNames = null;
        string FieldName(int id)
        {
            fieldNames ??= db.Set<CustomField>().AsNoTracking().ToDictionary(f => f.Id, f => f.Name);
            return fieldNames.GetValueOrDefault(id) ?? $"Custom field {id}";
        }

        AuditLog Row(string type, string key, string action, string? field, string? oldValue, string? newValue) => new()
        {
            Timestamp = now,
            UserName = Clip(user, 100),
            Source = Clip(source, 100) ?? "",
            EntityType = type,
            EntityKey = Clip(key, 100) ?? "",
            Action = action,
            Field = Clip(field, 100),
            OldValue = Clip(oldValue, MaxValue),
            NewValue = Clip(newValue, MaxValue)
        };

        foreach (var entry in entries)
        {
            // Custom-field values are fields of their ticket or card, so they
            // are recorded against it rather than as records of their own.
            if (entry.Entity is TransactionCustomValue tv)
            {
                var (o, n) = ValuePair(entry, nameof(TransactionCustomValue.Value));
                if (o != n) result.Add(new() { Entry = entry, Row = Row("Ticket", tv.Ticket, "Changed", FieldName(tv.CustomFieldId), o, n) });
                continue;
            }
            if (entry.Entity is CardCustomValue cv)
            {
                var (o, n) = ValuePair(entry, nameof(CardCustomValue.Value));
                if (o == n) continue;
                var cardNumber = db.Set<Card>().Local.FirstOrDefault(c => c.Id == cv.CardId)?.CardNumber
                                 ?? db.Set<Card>().AsNoTracking().Where(c => c.Id == cv.CardId).Select(c => c.CardNumber).FirstOrDefault()
                                 ?? cv.CardId.ToString(CultureInfo.InvariantCulture);
                result.Add(new() { Entry = entry, Row = Row("Card", cardNumber, "Changed", FieldName(cv.CustomFieldId), o, n) });
                continue;
            }

            var typeName = TypeName(entry.Entity.GetType());
            switch (entry.State)
            {
                case EntityState.Added:
                {
                    var keyLater = entry.Properties.Any(p => p.Metadata.IsPrimaryKey() && p.IsTemporary);
                    var row = Row(typeName, keyLater ? "" : KeyOf(entry), "Created", null, null, Summary(entry, current: true));
                    result.Add(new() { Entry = entry, Row = row, KeyAfterSave = keyLater });
                    break;
                }
                case EntityState.Deleted:
                    result.Add(new() { Entry = entry, Row = Row(typeName, KeyOf(entry, original: true), "Deleted", null, Summary(entry, current: false), null) });
                    break;
                case EntityState.Modified:
                {
                    var key = KeyOf(entry);
                    foreach (var prop in entry.Properties)
                    {
                        if (!prop.IsModified || Ignore(entry, prop.Metadata, path)) continue;
                        var o = Format(prop.Metadata, prop.OriginalValue);
                        var n = Format(prop.Metadata, prop.CurrentValue);
                        if (o == n) continue;
                        result.Add(new() { Entry = entry, Row = Row(typeName, key, "Changed", FieldLabel(prop.Metadata), o, n) });
                    }
                    break;
                }
            }
        }
        return result;
    }

    private static (string? Old, string? New) ValuePair(EntityEntry entry, string property)
    {
        var p = entry.Property(property);
        return entry.State switch
        {
            EntityState.Added => (null, Format(p.Metadata, p.CurrentValue)),
            EntityState.Deleted => (Format(p.Metadata, p.OriginalValue), null),
            _ => (Format(p.Metadata, p.OriginalValue), Format(p.Metadata, p.CurrentValue))
        };
    }

    private static bool Ignore(EntityEntry entry, IProperty prop, string path)
    {
        if (Ignored.Contains((entry.Entity.GetType(), prop.Name))) return true;
        // The next ticket number moves on with every ticket; only a change an
        // admin makes on the Setup page is worth recording.
        if (entry.Entity is AppSetup && prop.Name == nameof(AppSetup.TicketNumber))
            return !path.StartsWith("/Setup", StringComparison.OrdinalIgnoreCase);
        return false;
    }

    /// <summary>"Customer: Acme; In Weight: 40,000; …" — the non-empty values
    /// of a record created or deleted.</summary>
    private static string Summary(EntityEntry entry, bool current)
    {
        var sb = new StringBuilder();
        foreach (var prop in entry.Properties)
        {
            if (prop.Metadata.IsPrimaryKey() && prop.Metadata.ValueGenerated != ValueGenerated.Never) continue;
            if (Ignore(entry, prop.Metadata, "")) continue;
            var value = Format(prop.Metadata, current ? prop.CurrentValue : prop.OriginalValue);
            if (string.IsNullOrEmpty(value) || value == "No") continue;
            if (sb.Length > 0) sb.Append("; ");
            sb.Append(FieldLabel(prop.Metadata)).Append(": ").Append(value);
            if (sb.Length > MaxValue) break;
        }
        return sb.ToString();
    }

    private static string? Format(IProperty prop, object? value)
    {
        if (value == null) return null;
        // Passwords and secrets are never copied into the trail — only the
        // fact that one changed.
        if (value is string && prop.Name.Contains("Password", StringComparison.OrdinalIgnoreCase))
            return string.IsNullOrEmpty((string)value) ? null : "(hidden)";
        return value switch
        {
            bool b => b ? "Yes" : "No",
            DateTime d => AppTimeZone.FromUtc(d).ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
            byte[] bytes => bytes.Length == 0 ? null : "(file)",
            int i when IsWeight(prop) => i.ToString("#,##0", CultureInfo.InvariantCulture),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }

    private static bool IsWeight(IProperty prop) => prop.Name.EndsWith("Weight", StringComparison.Ordinal) || prop.Name == nameof(Truck.RetainedTare);

    /// <summary>The field's [Display] name, else its property name split into
    /// words ("InScale" → "In Scale").</summary>
    private static string FieldLabel(IProperty prop)
    {
        var display = prop.PropertyInfo?.GetCustomAttribute<DisplayAttribute>()?.Name;
        return string.IsNullOrEmpty(display) ? Words(prop.Name) : display;
    }

    private static string TypeName(Type t) => t.Name switch
    {
        nameof(Transaction) => "Ticket",
        nameof(AppSetup) => "Settings",
        nameof(AppUser) => "User",
        nameof(EmailSettings) => "Email Settings",
        _ => Words(t.Name)
    };

    /// <summary>How a reader would name the record: a ticket number, a
    /// customer's name, a card number. Falls back to the primary key.</summary>
    private static string KeyOf(EntityEntry entry, bool original = false)
    {
        string? V(string prop)
        {
            var p = entry.Property(prop);
            return (original ? p.OriginalValue : p.CurrentValue)?.ToString();
        }
        var key = entry.Entity switch
        {
            Transaction => V(nameof(Transaction.Ticket)),
            Customer => V(nameof(Customer.CustomerName)),
            Carrier => V(nameof(Carrier.CarrierName)),
            Commodity => V(nameof(Commodity.CommodityName)),
            Location => V(nameof(Location.LocationName)),
            Destination => V(nameof(Destination.DestinationName)),
            Bin => V(nameof(Bin.BinName)),
            Truck => $"{V(nameof(Truck.CarrierName))} / {V(nameof(Truck.TruckId))}",
            Card => V(nameof(Card.CardNumber)),
            AppUser => V(nameof(AppUser.Username)),
            AppSetup => "Setup",
            EmailSettings => "Email",
            Scale => V(nameof(Scale.Name)),
            Site => V(nameof(Site.Name)),
            Kiosk => V(nameof(Kiosk.Name)),
            CustomField => V(nameof(CustomField.Name)),
            PrintRule => V(nameof(PrintRule.Name)),
            _ => null
        };
        if (!string.IsNullOrEmpty(key)) return key;
        var pk = entry.Metadata.FindPrimaryKey();
        return pk == null
            ? ""
            : string.Join(", ", pk.Properties.Select(p => (original ? entry.Property(p.Name).OriginalValue : entry.Property(p.Name).CurrentValue)?.ToString()));
    }

    private static (string? User, string Source) Who(DbContext db, HttpContext? http, string path)
    {
        if (http == null) return (null, "System");

        var user = http.User?.Identity?.IsAuthenticated == true ? http.User.Identity!.Name : null;

        bool Under(string prefix) => path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
                                     || path.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase);

        string source;
        if (Under("/Kiosk") || Under("/api/kiosk"))
        {
            source = "Kiosk";
            var device = http.Request.Cookies[KioskController.DeviceCookie];
            if (!string.IsNullOrEmpty(device))
            {
                var name = db.Set<Kiosk>().AsNoTracking()
                    .Where(k => k.DeviceId == device).Select(k => k.Name).FirstOrDefault();
                if (!string.IsNullOrEmpty(name)) source = "Kiosk · " + name;
            }
        }
        else if (Under("/Mobile") || Under("/api/mobile")) source = "Phone";
        else if (Under("/SignaturePad") || Under("/api/signature")) source = "Signature Pad";
        else if (Under("/api/tables")) source = "API";
        else if ((path.StartsWith("/api/ticket/", StringComparison.OrdinalIgnoreCase) && path.EndsWith("/image", StringComparison.OrdinalIgnoreCase))
                 || Under("/api/transactions/mark-sent-to-qb") || Under("/api/masterdata/sync"))
            source = "Device Service";
        else source = "Office";

        return (user, source);
    }

    private static string Words(string name)
    {
        var sb = new StringBuilder(name.Length + 8);
        for (var i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1])) sb.Append(' ');
            sb.Append(name[i]);
        }
        return sb.ToString();
    }

    private static string? Clip(string? s, int max) =>
        s == null ? null : (s.Length <= max ? s : s[..(max - 1)] + "…");
}
