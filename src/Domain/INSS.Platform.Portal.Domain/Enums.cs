using System.ComponentModel;

namespace INSS.Platform.Portal.Domain;

public enum IncomeSource
{
    Salary,
    Benefits,
    Pensions,
    Rent,

    [Description("Dividends and investments")]
    Dividends,
}

public enum  PaymentFrequency
{
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Annually
}