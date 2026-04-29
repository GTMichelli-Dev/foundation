namespace BasicWeigh.Web.Services;

/// <summary>
/// Application-wide display timezone. Stored DateTimes are UTC; user-facing
/// display + date-range filtering use this configured TZ so behavior doesn't
/// depend on the host OS clock setting (cloud servers commonly run UTC).
///
/// Configured once at startup from appsettings.json: "Display:TimeZone"
/// (e.g. "America/Chicago"). Falls back to TimeZoneInfo.Local if unset or the
/// configured ID isn't found on the host.
/// </summary>
public static class AppTimeZone
{
    private static TimeZoneInfo _tz = TimeZoneInfo.Local;

    public static TimeZoneInfo Current => _tz;

    public static void Configure(string? tzId)
    {
        if (string.IsNullOrWhiteSpace(tzId)) return;
        try
        {
            _tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        }
        catch
        {
            // Bad ID or missing tzdata — keep TimeZoneInfo.Local as a fallback.
        }
    }

    /// <summary>
    /// Convert a DateTime expressed in the configured display TZ to UTC.
    /// Use for query-string date bounds before comparing to stored UTC values.
    /// </summary>
    public static DateTime ToUtc(DateTime localUnspecified)
    {
        var unspec = DateTime.SpecifyKind(localUnspecified, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspec, _tz);
    }

    /// <summary>
    /// Convert a stored UTC DateTime to the configured display TZ for
    /// server-side rendering (DevExpress reports, ticket prints).
    /// </summary>
    public static DateTime FromUtc(DateTime stored)
    {
        var asUtc = DateTime.SpecifyKind(stored, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(asUtc, _tz);
    }
}
