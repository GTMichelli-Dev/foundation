using Microsoft.AspNetCore.Mvc;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Controllers;

public class SetupController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly IConfiguration _config;

    public SetupController(ScaleDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public IActionResult Index()
    {
        var setup = _db.AppSetup.First();
        ViewBag.ShowResetDatabase = _config.GetValue<bool>("ShowResetDatabase", false);
        return View(setup);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(AppSetup setup, IFormFile? iconFile, bool removeIcon = false)
    {
        var existing = _db.AppSetup.Find(setup.Id);
        if (existing == null) return NotFound();

        existing.Header1 = setup.Header1;
        existing.Header2 = setup.Header2;
        existing.Header3 = setup.Header3;
        existing.Header4 = setup.Header4;
        existing.TicketNumber = setup.TicketNumber;
        existing.TicketsPerPage = setup.TicketsPerPage;
        existing.DemoMode = setup.DemoMode;
        existing.KioskCount = setup.KioskCount;
        existing.Theme = setup.Theme;
        existing.PromptKioskCommodity = setup.PromptKioskCommodity;
        existing.PromptKioskCustomer = setup.PromptKioskCustomer;
        existing.PromptKioskCarrier = setup.PromptKioskCarrier;
        existing.PromptKioskTruckId = setup.PromptKioskTruckId;
        existing.PromptKioskLocation = setup.PromptKioskLocation;
        existing.PromptKioskDestinationOnInbound = setup.PromptKioskDestinationOnInbound;
        existing.PromptKioskDestinationOnOutbound = setup.PromptKioskDestinationOnOutbound;
        existing.KioskDarkMode = setup.KioskDarkMode;
        existing.UseLogin = setup.UseLogin;
        existing.KioskCode = setup.KioskCode ?? "12345";
        existing.ApiDefinitionPin = setup.ApiDefinitionPin ?? "12345";
        existing.RecallLastValues = setup.RecallLastValues;
        existing.ScalePrintsTicket = setup.ScalePrintsTicket;

        if (removeIcon)
        {
            existing.Icon = null;
            existing.IconContentType = null;
        }
        else if (iconFile is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            iconFile.CopyTo(ms);
            existing.Icon = ms.ToArray();
            existing.IconContentType = iconFile.ContentType;
        }

        _db.SaveChanges();

        TempData["Message"] = "Settings saved successfully.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetTicket(string reportName)
    {
        var reportsDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        var path = Path.Combine(reportsDir, reportName + ".repx");
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        var label = reportName == "KioskTicketReport" ? "Kiosk Inbound Ticket" : "Completed Ticket";
        TempData["Message"] = $"{label} has been reset to default format.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearDatabase()
    {
        if (!_config.GetValue<bool>("ShowResetDatabase", false))
            return Forbid();

        _db.Transactions.RemoveRange(_db.Transactions);
        _db.Customers.RemoveRange(_db.Customers);
        _db.Carriers.RemoveRange(_db.Carriers);
        _db.Commodities.RemoveRange(_db.Commodities);
        _db.Locations.RemoveRange(_db.Locations);
        _db.Destinations.RemoveRange(_db.Destinations);
        _db.Trucks.RemoveRange(_db.Trucks);

        // Reset ticket number
        var setup = _db.AppSetup.First();
        setup.TicketNumber = 1;
        _db.SaveChanges();

        TempData["Message"] = "Database cleared. All transactions and master data removed.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LoadTestData()
    {
        if (!_config.GetValue<bool>("ShowResetDatabase", false))
            return Forbid();

        // Clear existing data first
        _db.Transactions.RemoveRange(_db.Transactions);
        _db.Customers.RemoveRange(_db.Customers);
        _db.Carriers.RemoveRange(_db.Carriers);
        _db.Commodities.RemoveRange(_db.Commodities);
        _db.Locations.RemoveRange(_db.Locations);
        _db.Destinations.RemoveRange(_db.Destinations);
        _db.Trucks.RemoveRange(_db.Trucks);
        _db.SaveChanges();

        // --- Master Data ---
        var customers = new[] { "Acme Gravel Co", "Smith Farms", "Delta Concrete", "Mountain Stone", "Valley Recycling" };
        foreach (var c in customers)
            _db.Customers.Add(new Customer { CustomerName = c, UseAtKiosk = true });

        var carriers = new[] { "Fast Haul Trucking", "River Road Transport", "Eagle Logistics", "Midwest Freight" };
        foreach (var c in carriers)
            _db.Carriers.Add(new Carrier { CarrierName = c, UseAtKiosk = true });

        var commodities = new[] { "Gravel", "Sand", "Topsoil", "Crushed Stone", "Recycled Asphalt", "Fill Dirt" };
        foreach (var c in commodities)
            _db.Commodities.Add(new Commodity { CommodityName = c, UseAtKiosk = true });

        var locations = new[] { "Pit A", "Pit B", "Yard 1", "Yard 2", "Stockpile" };
        foreach (var l in locations)
            _db.Locations.Add(new Location { LocationName = l, UseAtKiosk = true });

        var destinations = new[] { "Highway 50 Job", "Downtown Project", "Residential Site", "County Road 12", "Warehouse" };
        foreach (var d in destinations)
            _db.Destinations.Add(new Destination { DestinationName = d, UseAtKiosk = true });

        var trucks = new[]
        {
            ("T-101", "Fast Haul Trucking"), ("T-102", "Fast Haul Trucking"),
            ("R-200", "River Road Transport"), ("R-201", "River Road Transport"),
            ("E-300", "Eagle Logistics"),
            ("M-400", "Midwest Freight"), ("M-401", "Midwest Freight")
        };
        foreach (var (tid, carrier) in trucks)
            _db.Trucks.Add(new Truck { TruckId = tid, CarrierName = carrier, UseAtKiosk = true });

        _db.SaveChanges();

        // --- Transactions ---
        var rng = new Random(42); // Fixed seed for consistent data
        var now = DateTime.Now;
        int ticketNum = 1000;

        // Completed tickets (weighed in and out)
        var completedData = new[]
        {
            (cust: "Acme Gravel Co",  carr: "Fast Haul Trucking",    truck: "T-101", comm: "Gravel",          loc: "Pit A",     dest: "Highway 50 Job",     inW: 15200, outW: 42800, daysAgo: 5),
            (cust: "Smith Farms",     carr: "River Road Transport",  truck: "R-200", comm: "Topsoil",         loc: "Yard 1",    dest: "Residential Site",   inW: 14800, outW: 38600, daysAgo: 4),
            (cust: "Delta Concrete",  carr: "Eagle Logistics",       truck: "E-300", comm: "Sand",            loc: "Pit B",     dest: "Downtown Project",   inW: 16100, outW: 45200, daysAgo: 4),
            (cust: "Mountain Stone",  carr: "Midwest Freight",       truck: "M-400", comm: "Crushed Stone",   loc: "Stockpile", dest: "County Road 12",     inW: 15500, outW: 41900, daysAgo: 3),
            (cust: "Acme Gravel Co",  carr: "Fast Haul Trucking",    truck: "T-102", comm: "Gravel",          loc: "Pit A",     dest: "Highway 50 Job",     inW: 15000, outW: 43100, daysAgo: 3),
            (cust: "Valley Recycling",carr: "River Road Transport",  truck: "R-201", comm: "Recycled Asphalt",loc: "Yard 2",    dest: "Warehouse",          inW: 14600, outW: 39800, daysAgo: 2),
            (cust: "Smith Farms",     carr: "Fast Haul Trucking",    truck: "T-101", comm: "Fill Dirt",       loc: "Pit B",     dest: "Residential Site",   inW: 15300, outW: 44200, daysAgo: 2),
            (cust: "Delta Concrete",  carr: "Midwest Freight",       truck: "M-401", comm: "Sand",            loc: "Pit A",     dest: "Downtown Project",   inW: 16000, outW: 46100, daysAgo: 1),
            (cust: "Mountain Stone",  carr: "Eagle Logistics",       truck: "E-300", comm: "Crushed Stone",   loc: "Stockpile", dest: "County Road 12",     inW: 15700, outW: 42500, daysAgo: 1),
            (cust: "Acme Gravel Co",  carr: "Fast Haul Trucking",    truck: "T-101", comm: "Gravel",          loc: "Pit A",     dest: "Highway 50 Job",     inW: 15100, outW: 43800, daysAgo: 0),
        };

        foreach (var d in completedData)
        {
            ticketNum++;
            var dateIn = now.AddDays(-d.daysAgo).AddHours(rng.Next(6, 10)).AddMinutes(rng.Next(0, 60));
            _db.Transactions.Add(new Transaction
            {
                Ticket = ticketNum.ToString(),
                DateIn = dateIn,
                DateOut = dateIn.AddMinutes(rng.Next(20, 90)),
                InWeight = d.inW,
                OutWeight = d.outW,
                Customer = d.cust,
                Carrier = d.carr,
                TruckId = d.truck,
                Commodity = d.comm,
                Location = d.loc,
                Destination = d.dest,
                Void = false
            });
        }

        // Inbound tickets (weighed in, not yet out)
        ticketNum++;
        _db.Transactions.Add(new Transaction
        {
            Ticket = ticketNum.ToString(),
            DateIn = now.AddHours(-2),
            InWeight = 15400,
            Customer = "Valley Recycling",
            Carrier = "Midwest Freight",
            TruckId = "M-400",
            Commodity = "Recycled Asphalt",
            Location = "Yard 2",
            Void = false
        });

        ticketNum++;
        _db.Transactions.Add(new Transaction
        {
            Ticket = ticketNum.ToString(),
            DateIn = now.AddHours(-1),
            InWeight = 14900,
            Customer = "Smith Farms",
            Carrier = "River Road Transport",
            TruckId = "R-200",
            Commodity = "Topsoil",
            Location = "Yard 1",
            Void = false
        });

        // One voided ticket
        ticketNum++;
        _db.Transactions.Add(new Transaction
        {
            Ticket = ticketNum.ToString(),
            DateIn = now.AddDays(-3).AddHours(8),
            InWeight = 15600,
            Customer = "Delta Concrete",
            Carrier = "Eagle Logistics",
            TruckId = "E-300",
            Commodity = "Sand",
            Location = "Pit B",
            Void = true
        });

        ticketNum++;
        var setup = _db.AppSetup.First();
        setup.TicketNumber = ticketNum;
        _db.SaveChanges();

        TempData["Message"] = "Test data loaded: 10 completed, 2 inbound, and 1 voided ticket with master data.";
        return RedirectToAction("Index");
    }

    [HttpGet("api/setup/icon")]
    public IActionResult GetIcon()
    {
        var setup = _db.AppSetup.First();
        if (setup.Icon != null && setup.IconContentType != null)
            return File(setup.Icon, setup.IconContentType);

        // Fall back to default SVG
        var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default-icon.svg");
        return PhysicalFile(defaultPath, "image/svg+xml");
    }

    /// <summary>
    /// Returns the current system settings (excludes sensitive data like icon bytes).
    /// </summary>
    [HttpGet("api/setup/settings")]
    public IActionResult GetSettings()
    {
        var setup = _db.AppSetup.First();
        var version = GetType().Assembly.GetName().Version?.ToString() ?? "unknown";
        return Json(new
        {
            version,
            company = new
            {
                header1 = setup.Header1,
                header2 = setup.Header2,
                header3 = setup.Header3,
                header4 = setup.Header4
            },
            ticket = new
            {
                nextTicketNumber = setup.TicketNumber,
                ticketsPerPage = setup.TicketsPerPage
            },
            system = new
            {
                demoMode = setup.DemoMode,
                theme = setup.Theme,
                hasCustomIcon = setup.Icon != null,
                recallLastValues = setup.RecallLastValues
            },
            kiosk = new
            {
                kioskCount = setup.KioskCount,
                kioskDarkMode = setup.KioskDarkMode,
                promptCommodity = setup.PromptKioskCommodity,
                promptCustomer = setup.PromptKioskCustomer,
                promptCarrier = setup.PromptKioskCarrier,
                promptTruckId = setup.PromptKioskTruckId,
                promptLocation = setup.PromptKioskLocation,
                promptDestinationOnInbound = setup.PromptKioskDestinationOnInbound,
                promptDestinationOnOutbound = setup.PromptKioskDestinationOnOutbound
            },
            security = new
            {
                useLogin = setup.UseLogin
            }
        });
    }
}
