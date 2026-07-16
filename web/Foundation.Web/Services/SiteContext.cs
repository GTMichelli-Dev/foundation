using Foundation.Web.Data;
using Foundation.Web.Models;

namespace Foundation.Web.Services;

/// <summary>
/// The operator's current physical location (Site), chosen in the navbar and
/// remembered per browser via a cookie. Filters the scales — and optionally
/// bins/commodities — offered on the weigh forms, Get Weight tiles, and scale
/// pickers. Items with no SiteId are available everywhere; no cookie (or the
/// "All Locations" choice) means no filtering.
/// </summary>
public static class SiteContext
{
    public const string CookieName = "bw.siteId";

    /// <summary>Validated current site id, or null for "all locations".</summary>
    public static int? CurrentSiteId(HttpContext http, ScaleDbContext db)
    {
        var raw = http.Request.Cookies[CookieName];
        if (!int.TryParse(raw, out var id) || id <= 0) return null;
        return db.Sites.Any(s => s.Id == id && s.Active) ? id : null;
    }

    public static IQueryable<Scale> ForSite(this IQueryable<Scale> scales, int? siteId) =>
        siteId == null ? scales : scales.Where(s => s.SiteId == null || s.SiteId == siteId);

    public static IQueryable<Bin> ForSite(this IQueryable<Bin> bins, int? siteId) =>
        siteId == null ? bins : bins.Where(b => b.SiteId == null || b.SiteId == siteId);

    public static IQueryable<Commodity> ForSite(this IQueryable<Commodity> commodities, int? siteId) =>
        siteId == null ? commodities : commodities.Where(c => c.SiteId == null || c.SiteId == siteId);
}
