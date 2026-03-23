using GovUk.Forms.Domain.Attributes;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace GovUk.Forms.Domain;

public class DateModel : PageModel
{
    [Copyable]
    public int Day { get; set; } = 27;

    [Copyable]
    public int Month { get; set; } = 3;

    [Copyable]
    public int Year { get; set; } = 2001;
    
    [Summary]
    public DateOnly Value
    {
        get
        {
            string dateAsString = $"{Year:D4}-{Month:D2}-{Day:D2}";
            return DateOnly.TryParse(dateAsString, out DateOnly date) ? date : DateOnly.MinValue;
        }
    }
}