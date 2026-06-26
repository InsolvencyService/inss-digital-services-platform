// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace GovUk.Forms.Domain;

public class DateModel : PageModel
{
    public int Day { get; set; } = 27;

    public int Month { get; set; } = 3;

    public int Year { get; set; } = 2001;
    
    public DateOnly Value
    {
        get
        {
            return DateOnly.TryParse(DateAsString, out DateOnly date) ? date : DateOnly.MinValue;
        }
    }

    public string DateAsString => $"{Year:D4}-{Month:D2}-{Day:D2}";

    public override string[] GetSummaryInfo()
    {
        return [DateAsString];
    }

    public override void CopyTo(PageModel target)
    {
        DateModel date = target.As<DateModel>();
        date.Day = Day;
        date.Month = Month;
        date.Year = Year;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Day = 0;
        Month = 0;
        Year = 0;
    }
}