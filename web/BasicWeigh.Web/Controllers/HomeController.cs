using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Models;
using BasicWeigh.Web.Services;

namespace BasicWeigh.Web.Controllers;

public class HomeController : Controller
{
    private readonly IScaleService _scaleService;

    public HomeController(IScaleService scaleService)
    {
        _scaleService = scaleService;
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
            error = _scaleService.HasError()
        });
    }

    [HttpPost("api/scale/simulate")]
    public IActionResult Simulate([FromBody] SimulateRequest request)
    {
        if (_scaleService is SimulatedScaleService sim)
        {
            sim.SetWeight(request.Weight);
            sim.SetMotion(request.Motion);
            sim.SetError(request.Error);
            return Json(new { success = true });
        }
        return BadRequest(new { success = false, message = "Not in demo mode" });
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
