using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Controllers;

public class MasterDataController : Controller
{
    private readonly ScaleDbContext _db;

    public MasterDataController(ScaleDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    // ---- Customers ----
    [HttpGet("api/masterdata/customers")]
    public IActionResult GetCustomers()
    {
        return Json(_db.Customers.OrderBy(c => c.CustomerName).ToList());
    }

    [HttpPost("api/masterdata/customers")]
    public IActionResult AddCustomer([FromBody] Customer customer)
    {
        _db.Customers.Add(customer);
        _db.SaveChanges();
        return Json(customer);
    }

    [HttpPut("api/masterdata/customers")]
    public IActionResult UpdateCustomer([FromBody] Customer customer)
    {
        var existing = _db.Customers.Find(customer.CustomerName);
        if (existing == null) return NotFound();
        existing.Active = customer.Active;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/masterdata/customers/{key}")]
    public IActionResult DeleteCustomer(string key)
    {
        var entity = _db.Customers.Find(key);
        if (entity == null) return NotFound();
        _db.Customers.Remove(entity);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    // ---- Carriers ----
    [HttpGet("api/masterdata/carriers")]
    public IActionResult GetCarriers()
    {
        return Json(_db.Carriers.OrderBy(c => c.CarrierName).ToList());
    }

    [HttpPost("api/masterdata/carriers")]
    public IActionResult AddCarrier([FromBody] Carrier carrier)
    {
        _db.Carriers.Add(carrier);
        _db.SaveChanges();
        return Json(carrier);
    }

    [HttpPut("api/masterdata/carriers")]
    public IActionResult UpdateCarrier([FromBody] Carrier carrier)
    {
        var existing = _db.Carriers.Find(carrier.CarrierName);
        if (existing == null) return NotFound();
        existing.Active = carrier.Active;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/masterdata/carriers/{key}")]
    public IActionResult DeleteCarrier(string key)
    {
        var entity = _db.Carriers.Find(key);
        if (entity == null) return NotFound();
        _db.Carriers.Remove(entity);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    // ---- Locations ----
    [HttpGet("api/masterdata/locations")]
    public IActionResult GetLocations()
    {
        return Json(_db.Locations.OrderBy(l => l.LocationName).ToList());
    }

    [HttpPost("api/masterdata/locations")]
    public IActionResult AddLocation([FromBody] Location location)
    {
        _db.Locations.Add(location);
        _db.SaveChanges();
        return Json(location);
    }

    [HttpPut("api/masterdata/locations")]
    public IActionResult UpdateLocation([FromBody] Location location)
    {
        var existing = _db.Locations.Find(location.LocationName);
        if (existing == null) return NotFound();
        existing.Active = location.Active;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/masterdata/locations/{key}")]
    public IActionResult DeleteLocation(string key)
    {
        var entity = _db.Locations.Find(key);
        if (entity == null) return NotFound();
        _db.Locations.Remove(entity);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    // ---- Destinations ----
    [HttpGet("api/masterdata/destinations")]
    public IActionResult GetDestinations()
    {
        return Json(_db.Destinations.OrderBy(d => d.DestinationName).ToList());
    }

    [HttpPost("api/masterdata/destinations")]
    public IActionResult AddDestination([FromBody] Destination destination)
    {
        _db.Destinations.Add(destination);
        _db.SaveChanges();
        return Json(destination);
    }

    [HttpPut("api/masterdata/destinations")]
    public IActionResult UpdateDestination([FromBody] Destination destination)
    {
        var existing = _db.Destinations.Find(destination.DestinationName);
        if (existing == null) return NotFound();
        existing.Active = destination.Active;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/masterdata/destinations/{key}")]
    public IActionResult DeleteDestination(string key)
    {
        var entity = _db.Destinations.Find(key);
        if (entity == null) return NotFound();
        _db.Destinations.Remove(entity);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    // ---- Trucks ----
    [HttpGet("api/masterdata/trucks")]
    public IActionResult GetTrucks()
    {
        return Json(_db.Trucks.OrderBy(t => t.TruckId).ToList());
    }

    [HttpPost("api/masterdata/trucks")]
    public IActionResult AddTruck([FromBody] Truck truck)
    {
        _db.Trucks.Add(truck);
        _db.SaveChanges();
        return Json(truck);
    }

    [HttpPut("api/masterdata/trucks")]
    public IActionResult UpdateTruck([FromBody] Truck truck)
    {
        var existing = _db.Trucks.Find(truck.TruckId);
        if (existing == null) return NotFound();
        existing.Phone = truck.Phone;
        existing.Lot = truck.Lot;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/masterdata/trucks/{key}")]
    public IActionResult DeleteTruck(string key)
    {
        var entity = _db.Trucks.Find(key);
        if (entity == null) return NotFound();
        _db.Trucks.Remove(entity);
        _db.SaveChanges();
        return Json(new { success = true });
    }

    // ---- Commodities ----
    [HttpGet("api/masterdata/commodities")]
    public IActionResult GetCommodities()
    {
        return Json(_db.Commodities.OrderBy(c => c.CommodityName).ToList());
    }

    [HttpPost("api/masterdata/commodities")]
    public IActionResult AddCommodity([FromBody] Commodity commodity)
    {
        _db.Commodities.Add(commodity);
        _db.SaveChanges();
        return Json(commodity);
    }

    [HttpPut("api/masterdata/commodities")]
    public IActionResult UpdateCommodity([FromBody] Commodity commodity)
    {
        var existing = _db.Commodities.Find(commodity.CommodityName);
        if (existing == null) return NotFound();
        existing.Active = commodity.Active;
        _db.SaveChanges();
        return Json(existing);
    }

    [HttpDelete("api/masterdata/commodities/{key}")]
    public IActionResult DeleteCommodity(string key)
    {
        var entity = _db.Commodities.Find(key);
        if (entity == null) return NotFound();
        _db.Commodities.Remove(entity);
        _db.SaveChanges();
        return Json(new { success = true });
    }
}
