using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Foundation.Web.Data;
using Foundation.Web.Models;
using Foundation.Web.Services;

namespace Foundation.Web.Controllers;

/// <summary>
/// Cards: a card carries a load's details so nobody enters them at the scale.
///
///   /Card       — enrollment. A manager registers each card's number once
///                 (api/cardadmin/*). Only enrolled cards can be issued or used
///                 at a kiosk.
///   /Card/Setup — the loader operator's page (api/cards/*). Find the card,
///                 its last-issued values come back, edit what this load needs,
///                 save. The driver presents it at a reader or keys it in.
///   /Card/Bulk  — set fields on many cards at once (api/cardadmin/bulk).
/// </summary>
public class CardController : Controller
{
    private readonly ScaleDbContext _db;
    private readonly AppSetupCache _setupCache;

    public CardController(ScaleDbContext db, AppSetupCache setupCache)
    {
        _db = db;
        _setupCache = setupCache;
    }

    // ===== PAGES =====

    /// <summary>Card enrollment (Manager / Admin — enforced in Program.cs).</summary>
    public IActionResult Index()
    {
        var setup = _setupCache.Get();
        ViewBag.RecycleCards = setup.RecycleCards;
        ViewBag.UseCardReader = setup.UseCardReader;
        return View();
    }

    /// <summary>The loader operator's card-issue page (phone or desktop).</summary>
    public IActionResult Setup()
    {
        var setup = _setupCache.Get();
        ViewBag.RecycleCards = setup.RecycleCards;
        ViewBag.UseCardReader = setup.UseCardReader;
        ViewBag.UseRetainedTare = setup.UseRetainedTare;
        // Renumbering a card and bulk updates are enrollment jobs, so their
        // buttons only show to the roles allowed to use them.
        ViewBag.CanManageCards = CanManageCards(setup);
        return View();
    }

    /// <summary>Set fields on many cards at once (Manager / Admin — enforced in Program.cs).</summary>
    public IActionResult Bulk()
    {
        ViewBag.RecycleCards = _setupCache.Get().RecycleCards;
        return View();
    }

    // ===== OPERATOR API =====

    /// <summary>
    /// The fields a card can carry, in weigh-form order, with their choice
    /// lists and which of them the kiosk would refuse to skip.
    /// </summary>
    [HttpGet("api/cards/fields")]
    public IActionResult GetFields()
    {
        var siteId = SiteContext.CurrentSiteId(HttpContext, _db);
        var setup = _setupCache.Get();
        return Json(new
        {
            fields = CardFields.Describe(_db, setup, siteId),
            recycleCards = setup.RecycleCards
        });
    }

    /// <summary>
    /// Recall a card by its number. Returns the card's stored values so the
    /// operator only changes what this load needs. A card that isn't enrolled
    /// comes back found=false — enrollment is an admin job on /Card.
    /// </summary>
    [HttpGet("api/cards/lookup")]
    public IActionResult Lookup([FromQuery] string cardNumber)
    {
        var number = (cardNumber ?? "").Trim();
        if (number.Length == 0)
            return BadRequest(new { message = "Enter a card number." });

        var card = FindByNumber(number);
        if (card == null)
            return Ok(new { found = false, cardNumber = number });

        return Ok(new { found = true, card = Describe(card) });
    }

    /// <summary>
    /// The Weigh In page's card shortcut (Setup → Cards → Allow Cards on the
    /// Weigh Forms): what the card would do at a kiosk right now — fill in a
    /// weigh-in, or point at the open ticket it is already on.
    /// </summary>
    [HttpGet("api/cards/resolve")]
    public IActionResult Resolve([FromQuery] string? cardNumber, [FromQuery] int? scaleId)
    {
        var setup = _setupCache.Get();
        if (!setup.UseCardReader || !setup.AllowCardDesktop)
            return Ok(new { ok = false, reason = "disabled", message = "Cards are turned off on the weigh forms (Setup → Cards)." });

        // The office pages are English, so the messages are left as they are.
        var r = CardFields.Resolve(_db, setup, cardNumber, scaleId, k => k);
        if (!r.Ok)
            return Ok(new { ok = false, reason = r.Reason, message = r.Message });

        return Ok(new
        {
            ok = true,
            action = r.Action,
            card = r.Summary,
            values = r.Values,
            missingRequired = r.MissingRequired,
            ticket = r.Ticket == null ? null : new { ticket = r.Ticket.Ticket, carrier = r.Ticket.Carrier, truckId = r.Ticket.TruckId }
        });
    }

    /// <summary>
    /// Save the card's field values and hand it to the driver. Issuing a card
    /// that is mid-trip is refused — its open ticket would be orphaned.
    /// </summary>
    [HttpPost("api/cards/{id:int}/issue")]
    public IActionResult Issue(int id, [FromBody] IssueRequest body)
    {
        var card = _db.Cards.Find(id);
        if (card == null) return NotFound(new { message = "Card not found." });
        if (!card.Enabled)
            return BadRequest(new { message = "This card is disabled. Re-enable it on the Cards page first." });

        if (!string.IsNullOrEmpty(card.OpenTicket)
            && _db.Transactions.Any(t => t.Ticket == card.OpenTicket && !t.Void && t.DateOut == null))
        {
            return BadRequest(new
            {
                message = $"Card is on open ticket #{card.OpenTicket}. Weigh that load out before re-issuing."
            });
        }
        // Stale link (ticket voided or closed elsewhere) — clear it and carry on.
        card.OpenTicket = null;

        var setup = _setupCache.Get();
        var siteId = SiteContext.CurrentSiteId(HttpContext, _db);
        var descriptors = CardFields.Describe(_db, setup, siteId);
        var values = body.Values ?? new Dictionary<string, string?>();

        // Truck IDs are validated against the carrier being saved, which the
        // generic descriptor validation can't do without the database.
        if (values.TryGetValue("truck", out var truckId) && !string.IsNullOrWhiteSpace(truckId))
        {
            var carrier = values.GetValueOrDefault("carrier")?.Trim() ?? card.Carrier;
            var t = truckId.Trim();
            var known = !string.IsNullOrWhiteSpace(carrier)
                && _db.Trucks.Any(x => x.TruckId == t && x.CarrierName == carrier && x.UseAtKiosk);
            if (!known)
                return BadRequest(new { message = $"Truck \"{t}\" is not assigned to that carrier." });
        }

        CardFields.ApplyValues(_db, card, values, descriptors);

        if (body.RecycleMode is "Default" or "Recycle" or "Deactivate")
            card.RecycleMode = body.RecycleMode;

        card.Issued = true;
        card.IssuedAt = DateTime.UtcNow;
        card.IssuedBy = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
        _db.SaveChanges();

        // Missing-required is a warning, not a rejection: the kiosk will ask
        // the driver for anything the card doesn't answer.
        var stored = CardFields.ValuesOf(_db, card);
        var missing = descriptors
            .Where(d => d.Required && !stored.ContainsKey(d.Key))
            .Select(d => d.Label)
            .ToList();

        return Ok(new { card = Describe(card), missingRequired = missing });
    }

    /// <summary>Take a card out of service now (driver never came back, wrong
    /// card handed out). Refused while a ticket is open on it.</summary>
    [HttpPost("api/cards/{id:int}/release")]
    public IActionResult Release(int id)
    {
        var card = _db.Cards.Find(id);
        if (card == null) return NotFound(new { message = "Card not found." });

        if (!string.IsNullOrEmpty(card.OpenTicket)
            && _db.Transactions.Any(t => t.Ticket == card.OpenTicket && !t.Void && t.DateOut == null))
        {
            return BadRequest(new
            {
                message = $"Card is on open ticket #{card.OpenTicket}. Weigh that load out first."
            });
        }

        card.OpenTicket = null;
        card.Issued = false;
        card.IssuedAt = null;
        _db.SaveChanges();
        return Ok(new { card = Describe(card) });
    }

    /// <summary>Trucks for a carrier. The kiosk has its own copy of this at
    /// /api/kiosk/trucks, but that path is PIN-gated for unattended terminals —
    /// the card pages are behind a normal login.</summary>
    [HttpGet("api/cards/trucks")]
    public IActionResult GetTrucks([FromQuery] string carrier)
    {
        var c = (carrier ?? "").Trim();
        if (c.Length == 0) return Json(Array.Empty<string>());
        return Json(_db.Trucks
            .Where(t => t.CarrierName == c && t.UseAtKiosk)
            .OrderBy(t => t.TruckId)
            .Select(t => t.TruckId)
            .ToList());
    }

    /// <summary>
    /// Find cards for Card Setup's search: by number, description or any value
    /// a card carries (customer, carrier, truck, ...). Open to every role like
    /// the rest of api/cards — the loader operator uses it to find a card
    /// without its number to hand.
    /// </summary>
    [HttpGet("api/cards/search")]
    public IActionResult Search([FromQuery] string? q)
    {
        var term = (q ?? "").Trim();
        var matches = _db.Cards
            .OrderBy(c => c.CardNumber.Length).ThenBy(c => c.CardNumber)
            .ToList()
            .Where(c => term.Length == 0
                || c.CardNumber.Contains(term, StringComparison.OrdinalIgnoreCase)
                || (c.Description ?? "").Contains(term, StringComparison.OrdinalIgnoreCase)
                || CardFields.ValuesOf(_db, c).Values.Any(v => v.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Take(50)
            .Select(Describe)
            .ToList();
        return Json(matches);
    }

    // ===== ENROLLMENT API (Manager / Admin — enforced in Program.cs) =====

    [HttpGet("api/cardadmin")]
    public IActionResult ListCards()
    {
        var cards = _db.Cards
            .OrderByDescending(c => c.Issued)
            .ThenBy(c => c.CardNumber)
            .ToList()
            .Select(Describe)
            .ToList();
        return Json(cards);
    }

    // Cards live below the first ticket number so the kiosk keypad can tell a
    // keyed card from a keyed ticket.
    private static readonly string CardNumberRule =
        $"Card numbers are 1 to {TicketNumbers.Minimum - 1}. Numbers from {TicketNumbers.Minimum} up are ticket numbers.";

    [HttpPost("api/cardadmin")]
    public IActionResult Enroll([FromBody] EnrollRequest body)
    {
        var number = (body.CardNumber ?? "").Trim();
        if (number.Length == 0)
            return BadRequest(new { message = "Card number is required." });
        if (!TicketNumbers.IsCardNumber(number, out var normalized))
            return BadRequest(new { message = CardNumberRule });
        number = normalized;
        if (_db.Cards.Any(c => c.CardNumber == number))
            return BadRequest(new { message = $"Card \"{number}\" is already enrolled." });

        var card = new Card
        {
            CardNumber = number,
            Description = Trim(body.Description, 100),
            Enabled = body.Enabled ?? true,
            RecycleMode = Normalize(body.RecycleMode)
        };
        _db.Cards.Add(card);
        _db.SaveChanges();
        return Json(Describe(card));
    }

    [HttpPut("api/cardadmin/{id:int}")]
    public IActionResult UpdateCard(int id, [FromBody] EnrollRequest body)
    {
        var card = _db.Cards.Find(id);
        if (card == null) return NotFound(new { message = "Card not found." });

        var number = (body.CardNumber ?? "").Trim();
        if (number.Length == 0)
            return BadRequest(new { message = "Card number is required." });
        // A card enrolled before the rule keeps its number until it is changed.
        if (number != card.CardNumber)
        {
            if (!TicketNumbers.IsCardNumber(number, out var normalized))
                return BadRequest(new { message = CardNumberRule });
            number = normalized;
        }
        // A card mid-trip is matched to its ticket partly by number, so its
        // number stays put until that load weighs out.
        if (number != card.CardNumber && !string.IsNullOrEmpty(card.OpenTicket)
            && _db.Transactions.Any(t => t.Ticket == card.OpenTicket && !t.Void && t.DateOut == null))
        {
            return BadRequest(new
            {
                message = $"Card is on open ticket #{card.OpenTicket}. Weigh that load out before changing its number."
            });
        }
        if (_db.Cards.Any(c => c.Id != id && c.CardNumber == number))
            return BadRequest(new { message = $"Card \"{number}\" is already enrolled." });

        card.CardNumber = number;
        card.Description = Trim(body.Description, 100);
        if (body.Enabled.HasValue) card.Enabled = body.Enabled.Value;
        card.RecycleMode = Normalize(body.RecycleMode);

        // Disabling a card pulls it out of service immediately.
        if (!card.Enabled)
        {
            card.Issued = false;
            card.IssuedAt = null;
        }
        _db.SaveChanges();
        return Json(Describe(card));
    }

    [HttpDelete("api/cardadmin/{id:int}")]
    public IActionResult DeleteCard(int id)
    {
        var card = _db.Cards.Find(id);
        if (card == null) return NotFound(new { message = "Card not found." });

        if (!string.IsNullOrEmpty(card.OpenTicket)
            && _db.Transactions.Any(t => t.Ticket == card.OpenTicket && !t.Void && t.DateOut == null))
        {
            return BadRequest(new
            {
                message = $"Card is on open ticket #{card.OpenTicket}. Weigh that load out first."
            });
        }

        _db.Cards.Remove(card);
        _db.SaveChanges();
        return Ok(new { deleted = id });
    }

    /// <summary>
    /// Set field values on many cards at once (the Bulk Update page). Only the
    /// keys sent change — a blank value clears that field — and each card is
    /// checked the way Save &amp; Issue checks one: a value must be a choice the
    /// weigh forms allow, judged against that card's own answers, and a truck
    /// must belong to the card's carrier. Issued state is left alone; a card
    /// on an open ticket is skipped, as Issue would refuse it.
    /// </summary>
    [HttpPost("api/cardadmin/bulk")]
    public IActionResult BulkUpdate([FromBody] BulkRequest body)
    {
        var ids = (body.CardIds ?? new List<int>()).Distinct().ToList();
        var changes = body.Values ?? new Dictionary<string, string?>();
        var recycle = body.RecycleMode is "Default" or "Recycle" or "Deactivate" ? body.RecycleMode : null;
        if (ids.Count == 0)
            return BadRequest(new { message = "Select at least one card." });
        if (changes.Count == 0 && recycle == null)
            return BadRequest(new { message = "Tick at least one field to set." });

        var setup = _setupCache.Get();
        var siteId = SiteContext.CurrentSiteId(HttpContext, _db);
        var descriptors = CardFields.Describe(_db, setup, siteId);
        var labels = descriptors.ToDictionary(d => d.Key, d => d.Label);

        var skipped = new List<object>();
        var done = new List<(Card Card, List<string> NotApplied)>();

        foreach (var card in _db.Cards.Where(c => ids.Contains(c.Id)).ToList())
        {
            if (!string.IsNullOrEmpty(card.OpenTicket)
                && _db.Transactions.Any(t => t.Ticket == card.OpenTicket && !t.Void && t.DateOut == null))
            {
                skipped.Add(new { cardNumber = card.CardNumber, reason = $"on open ticket #{card.OpenTicket}" });
                continue;
            }

            // The card as it will be: its own values with the changes laid over,
            // so a cascading choice or a truck is judged against this card's
            // parent answer and carrier rather than whatever else was sent.
            var merged = CardFields.ValuesOf(_db, card)
                .ToDictionary(kv => kv.Key, kv => (string?)kv.Value);
            foreach (var kv in changes) merged[kv.Key] = kv.Value;

            var notApplied = new List<string>();
            if (changes.TryGetValue("truck", out var truck) && !string.IsNullOrWhiteSpace(truck))
            {
                var carrier = merged.GetValueOrDefault("carrier")?.Trim();
                var t = truck.Trim();
                var known = !string.IsNullOrWhiteSpace(carrier)
                    && _db.Trucks.Any(x => x.TruckId == t && x.CarrierName == carrier && x.UseAtKiosk);
                if (!known)
                {
                    merged.Remove("truck");
                    notApplied.Add(labels.GetValueOrDefault("truck", "Truck ID"));
                }
            }

            CardFields.ApplyValues(_db, card, merged, descriptors);
            if (recycle != null) card.RecycleMode = recycle;
            done.Add((card, notApplied));
        }
        _db.SaveChanges();

        // ApplyValues leaves out a value the weigh forms would refuse. Say which,
        // rather than let the operator think every card took it.
        var partial = new List<object>();
        foreach (var (card, notApplied) in done)
        {
            var after = CardFields.ValuesOf(_db, card);
            foreach (var kv in changes)
            {
                if (!labels.TryGetValue(kv.Key, out var label) || notApplied.Contains(label)) continue;
                var want = kv.Value?.Trim();
                var got = after.GetValueOrDefault(kv.Key);
                var took = string.IsNullOrEmpty(want) ? got == null : got == want;
                if (!took) notApplied.Add(label);
            }
            if (notApplied.Count > 0) partial.Add(new { cardNumber = card.CardNumber, fields = notApplied });
        }

        return Ok(new { updated = done.Count, skipped, partial });
    }

    // ===== HELPERS =====

    /// <summary>Manager or Admin, or anyone when login is off — the same test
    /// the navbar and Program.cs use for the enrollment pages.</summary>
    private bool CanManageCards(AppSetup setup)
    {
        if (!setup.UseLogin) return true;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return role is "Manager" or "Admin";
    }

    /// <summary>Card numbers are compared case-insensitively so a reader that
    /// emits lower-case hex still matches a number enrolled in upper case.</summary>
    private Card? FindByNumber(string number) =>
        _db.Cards.AsEnumerable()
            .FirstOrDefault(c => string.Equals(c.CardNumber, number, StringComparison.OrdinalIgnoreCase));

    private object Describe(Card card)
    {
        var setup = _setupCache.Get();
        var openTicketLive = !string.IsNullOrEmpty(card.OpenTicket)
            && _db.Transactions.Any(t => t.Ticket == card.OpenTicket && !t.Void && t.DateOut == null);

        return new
        {
            id = card.Id,
            cardNumber = card.CardNumber,
            description = card.Description,
            enabled = card.Enabled,
            issued = card.Issued,
            issuedAt = card.IssuedAt,
            issuedBy = card.IssuedBy,
            recycleMode = card.RecycleMode,
            recycles = card.RecyclesUnder(setup),
            openTicket = openTicketLive ? card.OpenTicket : null,
            lastTicket = card.LastTicket,
            lastUsedAt = card.LastUsedAt,
            values = CardFields.ValuesOf(_db, card)
        };
    }

    private static string? Trim(string? value, int max)
    {
        var v = value?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        return v.Length > max ? v[..max] : v;
    }

    private static string Normalize(string? recycleMode) =>
        recycleMode is "Recycle" or "Deactivate" ? recycleMode : "Default";

    public class IssueRequest
    {
        /// <summary>Field values keyed the same way the descriptors are
        /// ("commodity", "truck", "cf3"). A blank value clears the field.</summary>
        public Dictionary<string, string?>? Values { get; set; }
        public string? RecycleMode { get; set; }
    }

    public class EnrollRequest
    {
        public string? CardNumber { get; set; }
        public string? Description { get; set; }
        public bool? Enabled { get; set; }
        public string? RecycleMode { get; set; }
    }

    public class BulkRequest
    {
        public List<int>? CardIds { get; set; }
        /// <summary>Only the fields to change, keyed like IssueRequest.Values.
        /// A blank value clears the field.</summary>
        public Dictionary<string, string?>? Values { get; set; }
        /// <summary>Also set "After this load" when given.</summary>
        public string? RecycleMode { get; set; }
    }
}
