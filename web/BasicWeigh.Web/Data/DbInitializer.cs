using BasicWeigh.Web.Models;

namespace BasicWeigh.Web.Data;

public static class DbInitializer
{
    public static void Seed(ScaleDbContext context)
    {
        if (context.Customers.Any())
            return; // Already seeded

        // Customers
        var customers = new[]
        {
            new Customer { CustomerName = "ABC Construction", Active = true },
            new Customer { CustomerName = "Metro Recycling", Active = true },
            new Customer { CustomerName = "Greenfield Farms", Active = true },
            new Customer { CustomerName = "Riverside Aggregate", Active = true },
            new Customer { CustomerName = "Summit Materials", Active = true },
            new Customer { CustomerName = "Valley Sand & Gravel", Active = true },
            new Customer { CustomerName = "Lakeshore Paving", Active = true },
            new Customer { CustomerName = "Pioneer Demolition", Active = false },
        };
        context.Customers.AddRange(customers);

        // Carriers
        var carriers = new[]
        {
            new Carrier { CarrierName = "J&R Trucking", Active = true },
            new Carrier { CarrierName = "Eagle Transport", Active = true },
            new Carrier { CarrierName = "Midwest Haulers", Active = true },
            new Carrier { CarrierName = "FastFreight Inc", Active = true },
            new Carrier { CarrierName = "Big Rig Logistics", Active = true },
            new Carrier { CarrierName = "Smith & Sons Hauling", Active = false },
        };
        context.Carriers.AddRange(carriers);

        // Locations
        var locations = new[]
        {
            new Location { LocationName = "Main Yard", Active = true },
            new Location { LocationName = "North Pit", Active = true },
            new Location { LocationName = "South Quarry", Active = true },
            new Location { LocationName = "East Stockpile", Active = true },
            new Location { LocationName = "Warehouse A", Active = true },
        };
        context.Locations.AddRange(locations);

        // Destinations
        var destinations = new[]
        {
            new Destination { DestinationName = "Highway 12 Project", Active = true },
            new Destination { DestinationName = "Downtown Plaza", Active = true },
            new Destination { DestinationName = "Industrial Park", Active = true },
            new Destination { DestinationName = "Riverside Development", Active = true },
            new Destination { DestinationName = "County Landfill", Active = true },
            new Destination { DestinationName = "Municipal Yard", Active = true },
        };
        context.Destinations.AddRange(destinations);

        // Trucks
        var trucks = new[]
        {
            new Truck { TruckId = "TRK-101", Phone = "555-0101", Lot = "A" },
            new Truck { TruckId = "TRK-102", Phone = "555-0102", Lot = "A" },
            new Truck { TruckId = "TRK-205", Phone = "555-0205", Lot = "B" },
            new Truck { TruckId = "TRK-308", Phone = "555-0308", Lot = "B" },
            new Truck { TruckId = "TRK-410", Phone = "555-0410", Lot = "C" },
            new Truck { TruckId = "DMP-001", Phone = "555-0501", Lot = "A" },
            new Truck { TruckId = "DMP-002", Phone = "555-0502", Lot = "C" },
            new Truck { TruckId = "FLT-050", Phone = "555-0600", Lot = "B" },
        };
        context.Trucks.AddRange(trucks);

        // Commodities
        var commodities = new[]
        {
            new Commodity { CommodityName = "Gravel", Active = true },
            new Commodity { CommodityName = "Sand", Active = true },
            new Commodity { CommodityName = "Topsoil", Active = true },
            new Commodity { CommodityName = "Crushed Stone", Active = true },
            new Commodity { CommodityName = "Asphalt", Active = true },
            new Commodity { CommodityName = "Concrete Rubble", Active = true },
            new Commodity { CommodityName = "Fill Dirt", Active = true },
            new Commodity { CommodityName = "Scrap Metal", Active = true },
            new Commodity { CommodityName = "Mulch", Active = true },
        };
        context.Commodities.AddRange(commodities);

        context.SaveChanges();

        // Update ticket number for seeded transactions
        var setup = context.AppSetup.First();
        setup.TicketNumber = 1025;
        setup.Header1 = "Basic Weigh Scale";
        setup.Header2 = "123 Industrial Blvd";
        setup.Header3 = "Anytown, USA 12345";
        setup.Header4 = "(555) 123-4567";

        // Completed transactions (past 30 days)
        var random = new Random(42);
        var customerNames = customers.Where(c => c.Active).Select(c => c.CustomerName).ToArray();
        var carrierNames = carriers.Where(c => c.Active).Select(c => c.CarrierName).ToArray();
        var locationNames = locations.Where(l => l.Active).Select(l => l.LocationName).ToArray();
        var destNames = destinations.Where(d => d.Active).Select(d => d.DestinationName).ToArray();
        var truckIds = trucks.Select(t => t.TruckId).ToArray();
        var commodityNames = commodities.Where(c => c.Active).Select(c => c.CommodityName).ToArray();

        var ticketNum = 1000;
        var transactions = new List<Transaction>();

        // 40 completed transactions over the past 30 days
        for (int i = 0; i < 40; i++)
        {
            var dateIn = DateTime.Now.AddDays(-random.Next(1, 30)).AddHours(random.Next(6, 14)).AddMinutes(random.Next(0, 60));
            var dateOut = dateIn.AddMinutes(random.Next(15, 180));
            var inWeight = random.Next(15000, 45000);
            var outWeight = random.Next(8000, inWeight - 2000);

            transactions.Add(new Transaction
            {
                Ticket = (ticketNum++).ToString(),
                Void = i == 5, // one voided transaction
                InWeight = inWeight,
                DateIn = dateIn,
                DateOut = dateOut,
                OutWeight = outWeight,
                Customer = customerNames[random.Next(customerNames.Length)],
                Carrier = carrierNames[random.Next(carrierNames.Length)],
                TruckId = truckIds[random.Next(truckIds.Length)],
                Commodity = commodityNames[random.Next(commodityNames.Length)],
                Location = locationNames[random.Next(locationNames.Length)],
                Destination = destNames[random.Next(destNames.Length)],
                Comment = i % 5 == 0 ? "Load inspected" : null
            });
        }

        // 5 inbound trucks (in yard, not yet weighed out)
        for (int i = 0; i < 5; i++)
        {
            var dateIn = DateTime.Now.AddHours(-random.Next(1, 4)).AddMinutes(-random.Next(0, 60));
            transactions.Add(new Transaction
            {
                Ticket = (ticketNum++).ToString(),
                Void = false,
                InWeight = random.Next(18000, 42000),
                DateIn = dateIn,
                DateOut = null,
                OutWeight = null,
                Customer = customerNames[random.Next(customerNames.Length)],
                Carrier = carrierNames[random.Next(carrierNames.Length)],
                TruckId = truckIds[random.Next(truckIds.Length)],
                Commodity = commodityNames[random.Next(commodityNames.Length)],
                Location = locationNames[random.Next(locationNames.Length)],
                Destination = null,
                Comment = null
            });
        }

        context.Transactions.AddRange(transactions);
        setup.TicketNumber = ticketNum;
        context.SaveChanges();
    }
}
