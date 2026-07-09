namespace Foundation.Web.Services;

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
    /// Convert a stored UTC DateTime to the configured display TZ
    /// (AppTimeZone, "Display:TimeZone" in appsettings.json — defaults to
    /// host local). Use for server-side rendering (DevExpress reports,
    /// ticket prints) so the result doesn't depend on the host OS clock.
    /// </summary>
    public static DateTime ToServerLocal(this DateTime stored) =>
        AppTimeZone.FromUtc(stored);

    public static DateTime? ToServerLocal(this DateTime? stored) =>
        stored.HasValue ? AppTimeZone.FromUtc(stored.Value) : (DateTime?)null;
}
