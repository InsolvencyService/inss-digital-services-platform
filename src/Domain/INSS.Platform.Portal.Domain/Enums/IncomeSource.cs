using System.ComponentModel;

namespace INSS.Platform.Portal.Domain.Enums;

public enum IncomeSource
{
    Salary,
    Benefits,
    Pensions,
    Rent,

    [Description("Dividends and investments")]
    Dividends,
}
