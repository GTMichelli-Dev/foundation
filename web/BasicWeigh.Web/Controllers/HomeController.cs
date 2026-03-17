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
            motion = _scaleService.IsInMotion()
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
