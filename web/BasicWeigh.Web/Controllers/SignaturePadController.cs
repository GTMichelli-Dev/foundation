using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Services;

namespace BasicWeigh.Web.Controllers;

/// <summary>
/// Serves the standby page for a dedicated driver-signature tablet
/// (SignatureMode = RemotePad). The tablet opens
/// /SignaturePad?pad-id=xxx[&amp;pin=kioskPin] and waits on SignalR for
/// RequestSignature messages from the Weigh Out page.
/// Access is gated by the kiosk PIN middleware in Program.cs.
/// </summary>
public class SignaturePadController : Controller
{
    private readonly AppSetupCache _setupCache;

    public SignaturePadController(AppSetupCache setupCache)
    {
        _setupCache = setupCache;
    }

    public IActionResult Index()
    {
        var setup = _setupCache.Get();
        var padId = Request.Query["pad-id"].FirstOrDefault();
        ViewBag.PadId = string.IsNullOrWhiteSpace(padId) ? "default" : padId.Trim();
        ViewBag.Header1 = setup.Header1 ?? "Basic Weigh";
        return View();
    }
}
