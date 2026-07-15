using Microsoft.AspNetCore.Mvc;
using Foundation.Web.Data;
using Foundation.Web.Models;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

public class SetupController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly IConfiguration _config;
    private readonly AppSetupCache _setupCache;

    public SetupController(ScaleDbContext db, IConfiguration config, AppSetupCache setupCache)
    {
        _db = db;
        _config = config;
        _setupCache = setupCache;
    }

    public IActionResult Index()
    {
        var setup = _setupCache.Get();
        ViewBag.ShowResetDatabase = _config.GetValue<bool>("ShowResetDatabase", false);
        return View(setup);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(AppSetup setup, IFormFile? iconFile, bool removeIcon = false, string? activeTab = null)
    {
        var existing = _db.AppSetup.Find(setup.Id);
        if (existing == null) return NotFound();

        // Preserve the tab the operator was on so the redirect lands them back there.
        if (!string.IsNullOrEmpty(activeTab)) TempData["ActiveTab"] = activeTab;

        existing.Header1 = setup.Header1;
        existing.Header2 = setup.Header2;
        existing.Header3 = setup.Header3;
        existing.Header4 = setup.Header4;
        existing.TicketNumber = setup.TicketNumber;
        existing.TicketsPerPage = setup.TicketsPerPage;
        existing.DemoMode = setup.DemoMode;
        existing.KioskCount = setup.KioskCount;
        existing.Theme = setup.Theme;
        existing.PromptKioskCommodityOnInbound = setup.PromptKioskCommodityOnInbound;
        existing.PromptKioskCommodityOnOutbound = setup.PromptKioskCommodityOnOutbound;
        existing.AllowSkipCommodity = setup.AllowSkipCommodity;

        existing.PromptKioskCustomerOnInbound = setup.PromptKioskCustomerOnInbound;
        existing.PromptKioskCustomerOnOutbound = setup.PromptKioskCustomerOnOutbound;
        existing.AllowSkipCustomer = setup.AllowSkipCustomer;

        existing.PromptKioskCarrier = setup.PromptKioskCarrier;
        existing.AllowSkipCarrier = setup.AllowSkipCarrier;

        existing.PromptKioskLocationOnInbound = setup.PromptKioskLocationOnInbound;
        existing.PromptKioskLocationOnOutbound = setup.PromptKioskLocationOnOutbound;
        existing.AllowSkipLocation = setup.AllowSkipLocation;

        existing.PromptKioskTruckId = setup.PromptKioskTruckId;
        existing.AllowSkipTruckId = setup.AllowSkipTruckId;

        existing.PromptKioskDestinationOnInbound = setup.PromptKioskDestinationOnInbound;
        existing.PromptKioskDestinationOnOutbound = setup.PromptKioskDestinationOnOutbound;
        existing.AllowSkipDestination = setup.AllowSkipDestination;

        existing.HideKioskOnScreenButtons = setup.HideKioskOnScreenButtons;
        existing.TimeZoneId = string.IsNullOrWhiteSpace(setup.TimeZoneId) ? "America/Chicago" : setup.TimeZoneId;
        // Apply the new TZ to the running app immediately so subsequent
        // requests (and the redirect's GET render) use it without a restart.
        AppTimeZone.Configure(existing.TimeZoneId);
        existing.KioskDarkMode = setup.KioskDarkMode;
        existing.UseLogin = setup.UseLogin;
        existing.KioskCode = setup.KioskCode ?? "12345";
        existing.ApiDefinitionPin = setup.ApiDefinitionPin ?? "12345";
        existing.RecallLastValues = setup.RecallLastValues;
        existing.RemotePrintMode = setup.RemotePrintMode ?? "None";
        existing.UseQuickBooks = setup.UseQuickBooks;
        existing.SavePicture = setup.SavePicture;
        existing.UseRetainedTare = setup.UseRetainedTare;
        existing.AutoClearStaleRetainedTare = setup.AutoClearStaleRetainedTare;
        existing.SignatureMode = setup.SignatureMode ?? "None";
        existing.SignaturePadId = setup.SignaturePadId;
        existing.SignatureRequired = setup.SignatureRequired;
        existing.PrintSignatureOnTicket = setup.PrintSignatureOnTicket;

        existing.HideCustomer = setup.HideCustomer;
        existing.HideCarrier = setup.HideCarrier;
        existing.HideTruckId = setup.HideTruckId;
        existing.HideCommodity = setup.HideCommodity;
        existing.HideLocation = setup.HideLocation;
        existing.HideDestination = setup.HideDestination;
        existing.HideNotes = setup.HideNotes;

        existing.FieldOrderCommodity = setup.FieldOrderCommodity;
        existing.FieldOrderCustomer = setup.FieldOrderCustomer;
        existing.FieldOrderCarrier = setup.FieldOrderCarrier;
        existing.FieldOrderTruckId = setup.FieldOrderTruckId;
        existing.FieldOrderLocation = setup.FieldOrderLocation;
        existing.FieldOrderDestination = setup.FieldOrderDestination;
        existing.FieldOrderNotes = setup.FieldOrderNotes;

        // Field-visibility rules:
        // - Trucks are selected per carrier, so a visible Truck ID without a
        //   Carrier is unusable — hiding Carrier hides Truck ID too.
        // - Retained Tare identifies trucks by Carrier + Truck ID, so those two
        //   stay visible while it's enabled (mirrors the kiosk-prompt forcing).
        if (existing.HideCarrier) existing.HideTruckId = true;
        if (existing.UseRetainedTare)
        {
            existing.HideCarrier = false;
            existing.HideTruckId = false;
        }

        // Retained Tare needs both carrier and truck to identify the truck.
        // Force the prompts on AND disable Allow Skip so the kiosk flow always
        // captures the identifying fields with real values. Also force every
        // OnOutbound flag false — under Retained Tare the weigh-out is
        // auto-completed when the truck weighs in, so an outbound prompt would
        // never fire and would be a footgun if left checked.
        if (existing.UseRetainedTare)
        {
            existing.PromptKioskCarrier = true;
            existing.PromptKioskTruckId = true;
            existing.AllowSkipCarrier = false;
            existing.AllowSkipTruckId = false;

            existing.PromptKioskCommodityOnOutbound = false;
            existing.PromptKioskCustomerOnOutbound = false;
            existing.PromptKioskLocationOnOutbound = false;
            existing.PromptKioskDestinationOnOutbound = false;
        }

        // A hidden field must never be prompted for at the kiosk — force its
        // prompt flags off so the two settings can't contradict each other.
        // (Runs after the Retained Tare block; RT already forces Carrier and
        // Truck ID visible above, so there's no conflict.)
        if (existing.HideCustomer)
        {
            existing.PromptKioskCustomerOnInbound = false;
            existing.PromptKioskCustomerOnOutbound = false;
        }
        if (existing.HideCarrier) existing.PromptKioskCarrier = false;
        if (existing.HideTruckId) existing.PromptKioskTruckId = false;
        if (existing.HideCommodity)
        {
            existing.PromptKioskCommodityOnInbound = false;
            existing.PromptKioskCommodityOnOutbound = false;
        }
        if (existing.HideLocation)
        {
            existing.PromptKioskLocationOnInbound = false;
            existing.PromptKioskLocationOnOutbound = false;
        }
        if (existing.HideDestination)
        {
            existing.PromptKioskDestinationOnInbound = false;
            existing.PromptKioskDestinationOnOutbound = false;
        }
        // Camera assignments are managed on the Camera page and scales on the
        // Scales page (named site scales), so the posted AppSetup never carries
        // those fields — don't overwrite the saved values with the nulls that
        // come back from model binding. (ScaleId itself is legacy: the
        // AddMultiScale migration converted it into the first Scales row.)

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
        _setupCache.Invalidate();

        // Auto-save posts (fetch) get JSON back; a plain form post (no-JS
        // fallback) keeps the classic redirect + banner.
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true });

        TempData["Message"] = "Settings saved successfully.";
        return RedirectToAction("Index");
    }

    /// <summary>
    /// QR code (PNG) for the signature-pad URL so the tablet can scan instead
    /// of typing. Only same-app URLs are encoded — padId is the pad-id query
    /// value; the URL is built server-side from the current request host.
    /// </summary>
    [HttpGet("api/setup/signature-pad-qr")]
    public IActionResult SignaturePadQr(string? padId = null)
    {
        var url = $"{Request.Scheme}://{Request.Host}/SignaturePad?pad-id={Uri.EscapeDataString(string.IsNullOrWhiteSpace(padId) ? "default" : padId)}";
        using var generator = new QRCoder.QRCodeGenerator();
        using var data = generator.CreateQrCode(url, QRCoder.QRCodeGenerator.ECCLevel.M);
        var png = new QRCoder.PngByteQRCode(data).GetGraphic(pixelsPerModule: 5);
        return File(png, "image/png");
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
        _setupCache.Invalidate();

        TempData["Message"] = "Database cleared. All transactions and master data removed.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LoadTestData()
    {
        if (!_config.GetValue<bool>("ShowResetDatabase", false))
            return Forbid();

        // Clear everything
        _db.Transactions.RemoveRange(_db.Transactions);
        _db.Customers.RemoveRange(_db.Customers);
        _db.Carriers.RemoveRange(_db.Carriers);
        _db.Commodities.RemoveRange(_db.Commodities);
        _db.Locations.RemoveRange(_db.Locations);
        _db.Destinations.RemoveRange(_db.Destinations);
        _db.Trucks.RemoveRange(_db.Trucks);
        _db.SaveChanges();

        // --- Master Data (matching QuickBooks sample data) ---
        var customerNames = new[]
        {
            "ABC Construction LLC", "Greenfield Landscaping", "City of Springfield - Public Works",
            "Riverstone Builders", "Martin Paving Co", "Horizon Development Group",
            "Lakeview Concrete", "Prairie Homebuilders", "Tri-County Excavating",
            "Heartland Asphalt", "Sangamon County Highway Dept", "Cornerstone Ready Mix",
            "Iron Bridge Contractors", "Flatland Grading", "Stonewall Erosion Control",
            "Capital City Concrete", "Aggregate Haulers Midwest", "Lakeside Site Development"
        };
        foreach (var c in customerNames)
            _db.Customers.Add(new Customer { CustomerName = c, Active = true, UseAtKiosk = true });

        var commodityNames = new[]
        {
            "Fill Sand", "Mason Sand", "Concrete Sand", "Pea Gravel",
            "3/4 Crushed Stone", "1-1/2 Crushed Stone", "CA-6 Grade 8 Limestone",
            "CA-7 Chip Rock", "Rip Rap", "Screened Topsoil", "Road Gravel", "Recycled Concrete",
            "Hauling Fee"
        };
        foreach (var c in commodityNames)
            _db.Commodities.Add(new Commodity { CommodityName = c, Active = true, UseAtKiosk = true });

        var carrierData = new[]
        {
            "ABC Construction LLC", "Martin Paving Co", "Tri-County Excavating",
            "Flatland Grading", "Aggregate Haulers Midwest", "Heartland Asphalt",
            "River Road Transport", "Central IL Trucking", "Prairie State Hauling"
        };
        foreach (var c in carrierData)
            _db.Carriers.Add(new Carrier { CarrierName = c, Active = true, UseAtKiosk = true });

        var truckData = new (string TruckId, string Carrier)[]
        {
            ("ABC-101", "ABC Construction LLC"), ("ABC-102", "ABC Construction LLC"),
            ("MP-201", "Martin Paving Co"), ("MP-202", "Martin Paving Co"),
            ("TC-301", "Tri-County Excavating"), ("TC-302", "Tri-County Excavating"), ("TC-303", "Tri-County Excavating"),
            ("FG-401", "Flatland Grading"), ("FG-402", "Flatland Grading"),
            ("AH-501", "Aggregate Haulers Midwest"), ("AH-502", "Aggregate Haulers Midwest"), ("AH-503", "Aggregate Haulers Midwest"),
            ("HA-601", "Heartland Asphalt"), ("HA-602", "Heartland Asphalt"),
            ("RR-701", "River Road Transport"), ("RR-702", "River Road Transport"), ("RR-703", "River Road Transport"),
            ("CI-801", "Central IL Trucking"), ("CI-802", "Central IL Trucking"),
            ("PS-901", "Prairie State Hauling"), ("PS-902", "Prairie State Hauling")
        };
        foreach (var (tid, carrier) in truckData)
            _db.Trucks.Add(new Truck { TruckId = tid, CarrierName = carrier, UseAtKiosk = true });

        var locationNames = new[] { "Pit A - North", "Pit B - South", "Stockpile Yard", "Wash Plant", "Crusher" };
        foreach (var l in locationNames)
            _db.Locations.Add(new Location { LocationName = l, Active = true, UseAtKiosk = true });

        var destinationNames = new[]
        {
            "Highway 50 Job Site", "Downtown Springfield Project", "Chatham Subdivision",
            "County Road 12 Repair", "Lincoln Acres Development", "Riverton Bridge",
            "Industrial Park West", "Airport Expansion"
        };
        foreach (var d in destinationNames)
            _db.Destinations.Add(new Destination { DestinationName = d, Active = true, UseAtKiosk = true });

        _db.SaveChanges();

        // --- System settings: Demo mode, 1 kiosk, light mode ---
        var setup = _db.AppSetup.First();
        setup.DemoMode = true;
        setup.KioskCount = 1;
        setup.KioskDarkMode = false;

        // --- Build lookup lists for transactions ---
        var customers = customerNames.ToList();
        var commodities = commodityNames.ToList();
        var carriers = carrierData.ToList();
        var locations = locationNames.ToList();
        var destinations = destinationNames.ToList();
        var trucks = truckData.ToList();

        var rng = new Random(42);
        var now = DateTime.Now;
        int ticketNum = 1000;
        int completedCount = 0, voidedCount = 0;

        // Generate ~1 year of completed tickets, ~20 per weekday, 8am-5pm
        var startDate = now.AddYears(-1).Date;
        var endDate = now.Date;

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                continue;

            int ticketsToday = rng.Next(17, 24);
            for (int i = 0; i < ticketsToday; i++)
            {
                ticketNum++;
                var customer = customers[rng.Next(customers.Count)];
                var commodity = commodities[rng.Next(commodities.Count)];
                var carrier = carriers[rng.Next(carriers.Count)];

                // Randomly omit location and destination (30% chance each)
                string? location = rng.NextDouble() < 0.3 ? null : locations[rng.Next(locations.Count)];
                string? destination = rng.NextDouble() < 0.3 ? null : destinations[rng.Next(destinations.Count)];

                // Pick a truck for this carrier
                var carrierTrucks = trucks.Where(t => t.Carrier == carrier).ToList();
                var truckId = carrierTrucks.Count > 0
                    ? carrierTrucks[rng.Next(carrierTrucks.Count)].TruckId
                    : "T-" + rng.Next(100, 999);

                // Date in: 8am-4pm (leaves room for out time before 5pm)
                var dateIn = date.AddHours(rng.Next(8, 16)).AddMinutes(rng.Next(0, 60));
                // Date out: 5-20 min after date in
                var dateOut = dateIn.AddMinutes(rng.Next(5, 21));

                // Weights: one is tare (9000-25000), the other is gross (higher)
                // Net is ~30% of the larger weight
                var tareWeight = rng.Next(9000, 25001);
                var netWeight = (int)(tareWeight * (0.25 + rng.NextDouble() * 0.15)); // 25-40% of tare
                var grossWeight = tareWeight + netWeight;

                // Randomly inbound > outbound (~40% of time, e.g. dumping material)
                int inWeight, outWeight;
                if (rng.NextDouble() < 0.4)
                {
                    // Inbound is heavy (loaded coming in, empty going out)
                    inWeight = grossWeight;
                    outWeight = tareWeight;
                }
                else
                {
                    // Outbound is heavy (picking up material)
                    inWeight = tareWeight;
                    outWeight = grossWeight;
                }

                var isVoid = rng.NextDouble() < 0.005;

                _db.Transactions.Add(new Transaction
                {
                    Ticket = ticketNum.ToString(),
                    DateIn = dateIn,
                    DateOut = dateOut,
                    InWeight = inWeight,
                    OutWeight = outWeight,
                    Customer = customer,
                    Carrier = carrier,
                    TruckId = truckId,
                    Commodity = commodity,
                    Location = location,
                    Destination = destination,
                    Void = isVoid
                });

                if (isVoid) voidedCount++;
                else completedCount++;
            }
        }

        // Add 5 inbound (trucks in yard) tickets
        for (int i = 0; i < 5; i++)
        {
            ticketNum++;
            var customer = customers[rng.Next(customers.Count)];
            var commodity = commodities[rng.Next(commodities.Count)];
            var carrier = carriers[rng.Next(carriers.Count)];
            string? location = rng.NextDouble() < 0.3 ? null : locations[rng.Next(locations.Count)];
            string? destination = rng.NextDouble() < 0.3 ? null : destinations[rng.Next(destinations.Count)];
            var carrierTrucks = trucks.Where(t => t.Carrier == carrier).ToList();
            var truckId = carrierTrucks.Count > 0
                ? carrierTrucks[rng.Next(carrierTrucks.Count)].TruckId
                : "T-" + rng.Next(100, 999);

            _db.Transactions.Add(new Transaction
            {
                Ticket = ticketNum.ToString(),
                DateIn = now.AddMinutes(-(rng.Next(10, 180))),
                InWeight = rng.Next(9000, 110001),
                Customer = customer,
                Carrier = carrier,
                TruckId = truckId,
                Commodity = commodity,
                Location = location,
                Destination = destination,
                Void = false
            });
        }

        ticketNum++;
        setup.TicketNumber = ticketNum;
        _db.SaveChanges();
        _setupCache.Invalidate();

        TempData["Message"] = $"Test data loaded: {completedCount} completed, 5 in yard, and {voidedCount} voided tickets. Demo mode enabled with 1 kiosk (light).";
        return RedirectToAction("Index");
    }

    [HttpGet("api/setup/icon")]
    public IActionResult GetIcon()
    {
        var setup = _setupCache.Get();
        if (setup.Icon != null && setup.IconContentType != null)
            return File(setup.Icon, setup.IconContentType);

        // Fall back to default SVG
        var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default-icon.svg");
        return PhysicalFile(defaultPath, "image/svg+xml");
    }

    // Camera settings endpoint moved to CameraController

    /// <summary>
    /// Returns the current system settings (excludes sensitive data like icon bytes).
    /// </summary>
    [HttpGet("api/setup/settings")]
    public IActionResult GetSettings()
    {
        var setup = _setupCache.Get();
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
                promptCommodityOnInbound = setup.PromptKioskCommodityOnInbound,
                promptCommodityOnOutbound = setup.PromptKioskCommodityOnOutbound,
                allowSkipCommodity = setup.AllowSkipCommodity,
                promptCustomerOnInbound = setup.PromptKioskCustomerOnInbound,
                promptCustomerOnOutbound = setup.PromptKioskCustomerOnOutbound,
                allowSkipCustomer = setup.AllowSkipCustomer,
                promptCarrier = setup.PromptKioskCarrier,
                allowSkipCarrier = setup.AllowSkipCarrier,
                promptTruckId = setup.PromptKioskTruckId,
                allowSkipTruckId = setup.AllowSkipTruckId,
                promptLocationOnInbound = setup.PromptKioskLocationOnInbound,
                promptLocationOnOutbound = setup.PromptKioskLocationOnOutbound,
                allowSkipLocation = setup.AllowSkipLocation,
                promptDestinationOnInbound = setup.PromptKioskDestinationOnInbound,
                promptDestinationOnOutbound = setup.PromptKioskDestinationOnOutbound,
                allowSkipDestination = setup.AllowSkipDestination
            },
            security = new
            {
                useLogin = setup.UseLogin
            }
        });
    }
}
