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
    
    public DateOnly Value
    {
        get
        {
            return DateOnly.TryParse(DateAsString, out DateOnly date) ? date : DateOnly.MinValue;
        }
    }

    private string DateAsString => $"{Year:D4}-{Month:D2}-{Day:D2}";

    public override string[] GetSummaryInfo()
    {
        return [DateAsString];
    }
}