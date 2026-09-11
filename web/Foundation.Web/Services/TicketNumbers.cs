namespace Foundation.Web.Services;

/// <summary>
/// Ticket numbers and card numbers share one keypad at the kiosk, so they
/// share one number line: cards are 1–99999 and tickets start at 100000. A
/// number keyed in below the first ticket is a card; at or above it, a ticket.
/// </summary>
public static class TicketNumbers
{
    /// <summary>The first ticket number, and one past the highest card number.</summary>
    public const int Minimum = 100000;

    /// <summary>Raise a ticket counter to the minimum. Never lowers one.</summary>
    public static int Floor(int next) => Math.Max(next, Minimum);

    /// <summary>
    /// Whether a card number fits below the first ticket: digits only, 1–99999.
    /// <paramref name="normalized"/> drops leading zeros — readers send the
    /// number without them, so an enrolled "00123" would never match a
    /// presented 123.
    /// </summary>
    public static bool IsCardNumber(string? number, out string normalized)
    {
        normalized = "";
        var s = (number ?? "").Trim();
        if (s.Length == 0 || s.Length > 9 || !s.All(char.IsAsciiDigit)) return false;
        var n = int.Parse(s);
        if (n < 1 || n >= Minimum) return false;
        normalized = n.ToString();
        return true;
    }
}
