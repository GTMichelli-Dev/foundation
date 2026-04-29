namespace BasicWeigh.Web.Services;

/// <summary>
/// Date helpers for the Transaction.DateIn/DateOut fields. Stored values are UTC
/// (writes go through DateTime.UtcNow). EF / SQLite returns them with Kind =
/// Unspecified, so callers must restore the Kind before any conversion or JSON
/// serialization or the browser/.NET will misinterpret the value.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Stamp Kind = Utc on a stored DateTime so System.Text.Json emits the ISO
    /// string with a 'Z' suffix and the browser's <c>new Date()</c> + toLocaleString
    /// converts it to the user's timezone correctly.
    /// </summary>
    public static DateTime AsUtc(this DateTime stored) =>
        DateTime.SpecifyKind(stored, DateTimeKind.Utc);

    public static DateTime? AsUtc(this DateTime? stored) =>
        stored.HasValue ? DateTime.SpecifyKind(stored.Value, DateTimeKind.Utc) : (DateTime?)null;

    /// <summary>
    /// Convert a stored UTC DateTime to the server's local timezone for
    /// server-side rendering (DevExpress reports, ticket prints). The Linux
    /// host's TZ should be set via <c>timedatectl set-timezone</c>.
    /// </summary>
    public static DateTime ToServerLocal(this DateTime stored) =>
        DateTime.SpecifyKind(stored, DateTimeKind.Utc).ToLocalTime();

    public static DateTime? ToServerLocal(this DateTime? stored) =>
        stored.HasValue ? stored.Value.ToServerLocal() : (DateTime?)null;
}
