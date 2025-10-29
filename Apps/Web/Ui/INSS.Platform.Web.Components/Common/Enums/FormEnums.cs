using System.ComponentModel;

namespace INSS.Platform.Web.Components.Common.Enums
{
    /// <summary>
    /// Specifies the frequency types for payments.
    /// </summary>
    public enum PaymentFrequencyType
    {
        /// <summary>
        /// Payment is made every week.
        /// </summary>
        [Description("Weekly")]
        Weekly,

        /// <summary>
        /// Payment is made every month.
        /// </summary>
        [Description("Monthly")]
        Monthly,

        /// <summary>
        /// Payment is made on an ad-hoc basis.
        /// </summary>
        [Description("Ad-Hoc")]
        AdHoc
    }

    /// <summary>
    /// Specifies the types of companies.
    /// </summary>
    public enum CompanyType
    {
        /// <summary>
        /// A company that is registered as a limited company.
        /// </summary>
        [Description("Limited Company")]
        LimitedCompany,

        /// <summary>
        /// A company that is registered as a charity.
        /// </summary>
        [Description("Registered Charity")]
        RegisteredCharity,

        /// <summary>
        /// Any other type of company not listed.
        /// </summary>
        [Description("Other")]
        Other
    }

    /// <summary>
    /// Specifies a Yes or No value.
    /// </summary>
    public enum ConfirmType
    {
        /// <summary>
        /// Represents a 'Yes' response.
        /// </summary>
        [Description("Yes")]
        Yes,

        /// <summary>
        /// Represents a 'No' response.
        /// </summary>
        [Description("No")]
        No
    }

    /// <summary>
    /// Specifies a Yes or No value.
    /// </summary>
    public enum YesNoType
    {
        /// <summary>
        /// Represents a 'Yes' response.
        /// </summary>
        [Description("Yes")]
        Yes,

        /// <summary>
        /// Represents a 'No' response.
        /// </summary>
        [Description("No")]
        No
    }

    /// <summary>
    /// Specifies the types of income.
    /// </summary>
    public enum IncomeType
    {
        /// <summary>
        /// Income received from wages.
        /// </summary>
        [Description("Wages")]
        Wages,

        /// <summary>
        /// Income received from benefits.
        /// </summary>
        [Description("Benefits")]
        Benefits,

        /// <summary>
        /// Income received from a pension.
        /// </summary>
        [Description("Pension")]
        Pension,

        /// <summary>
        /// Income received from other sources.
        /// </summary>
        [Description("Other")]
        Other
    }

    public enum IncomeFrequencyType
    {
        /// <summary>
        /// Income is received hourly.
        /// </summary>
        [Description("Hourly")]
        Hourly,

        /// <summary>
        /// Income is received daily.
        /// </summary>
        [Description("Daily")]
        Daily,

        /// <summary>
        /// Income is received weekly.
        /// </summary>
        [Description("Weekly")]
        Weekly,

        /// <summary>
        /// Income is received monthly.
        /// </summary>
        [Description("Monthly")]
        Monthly,

        /// <summary>
        /// Income is received annually.
        /// </summary>
        [Description("Annually")]
        Annually
    }
}
