namespace GovUk.Forms.HostApp.UI.Test.Models;

public sealed record ArrearsOfPayPeriod(
 int PeriodNumber,
 string StartDate,
 string EndDate,
 string AmountOwed,
 string PayType);
