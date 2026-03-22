using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Services;

namespace BasicWeigh.Web.Controllers;

public class HomeController : Controller
{
    private readonly IScaleService _scaleService;
    private readonly ScaleDbContext _db;
    private readonly PrintQueueService _printQueue;

    public HomeController(IScaleService scaleService, ScaleDbContext db, PrintQueueService printQueue)
    {
        _scaleService = scaleService;
        _db = db;
        _printQueue = printQueue;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("api/scale/weight")]
    public IActionResult GetWeight()
    {
        return Json(new
        {
            weight = _scaleService.GetCurrentWeight(),
            motion = _scaleService.IsInMotion(),
            ok = _scaleService.IsConnected(),
            error = _scaleService.HasError(),
            comError = (_scaleService is SimulatedScaleService sim2) && sim2.HasComError()
        });
    }

    [HttpPost("api/scale/simulate")]
    public IActionResult Simulate([FromBody] SimulateRequest request)
    {
        var setup = _db.AppSetup.First();
        if (!setup.DemoMode)
            return BadRequest(new { success = false, message = "Not in demo mode. Enable Demo Mode in Setup to use the simulator." });

        if (_scaleService is SimulatedScaleService sim)
        {
            sim.SetMotion(request.Motion);
            sim.SetError(request.Error);
            return Json(new { success = true });
        }
        return BadRequest(new { success = false, message = "Scale service is not a simulator." });
    }

    /// <summary>
    /// Called by the physical scale indicator (or Pi bridge) to push weight readings.
    /// Only works when DemoMode is OFF. Returns any pending print job in the response.
    /// </summary>
    [HttpPost("api/scale/weight")]
    public IActionResult UpdateWeight([FromBody] ScaleWeightRequest request)
    {
        var setup = _db.AppSetup.First();
        if (setup.DemoMode)
            return BadRequest(new { success = false, message = "System is in demo mode. Use api/scale/simulate instead." });

        if (_scaleService is SimulatedScaleService sim)
        {
            sim.SetWeight(request.Weight);
            sim.SetMotion(request.Motion);
            sim.SetError(request.Error);
            sim.SetComError(false);
            sim.Touch();

            // Check for pending print jobs (Scale mode only — RemotePrinter uses SignalR)
            if (setup.RemotePrintMode == "Scale" && _printQueue.TryDequeue(out var ticketId) && ticketId != null)
            {
                return Json(new
                {
                    success = true,
                    print = new { ticketId, pdfUrl = $"/api/ticket/{ticketId}/pdf" }
                });
            }

            return Json(new { success = true });
        }

        return BadRequest(new { success = false, message = "Scale service unavailable." });
    }

    public class ScaleWeightRequest
    {
        public int Weight { get; set; }
        public bool Motion { get; set; }
        public bool Error { get; set; }
    }

    public class SimulateRequest
    {
        public int Weight { get; set; }
        public bool Motion { get; set; }
        public bool Error { get; set; }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
