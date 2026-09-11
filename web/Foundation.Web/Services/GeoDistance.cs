namespace Foundation.Web.Services;

/// <summary>
/// Distance between two GPS points, for keeping the mobile app to the scale
/// the phone is standing at. Haversine on a spherical earth: at the tens of
/// metres this is used for, the error is well under a metre — far smaller
/// than a phone's own GPS uncertainty.
/// </summary>
public static class GeoDistance
{
    private const double EarthRadiusMeters = 6_371_000;

    /// <summary>Metres between two points given in decimal degrees.</summary>
    public static double Meters(double lat1, double lng1, double lat2, double lng2)
    {
        static double Rad(double degrees) => degrees * Math.PI / 180;

        var dLat = Rad(lat2 - lat1);
        var dLng = Rad(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2)) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        return 2 * EarthRadiusMeters * Math.Asin(Math.Min(1, Math.Sqrt(a)));
    }

    /// <summary>Whether a latitude/longitude pair is a real position.</summary>
    public static bool IsValid(double? lat, double? lng) =>
        lat is >= -90 and <= 90 && lng is >= -180 and <= 180
        // 0,0 is in the Gulf of Guinea: a coordinate nobody meant to enter.
        && !(lat == 0 && lng == 0);
}
