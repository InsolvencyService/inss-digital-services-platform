namespace Inss.GovUk.Forms.IPUpload.Domain;

public static class CaseReferenceAnnotation
{
    public const string Category = "Case";
    public const string PropertyName = "Case reference";
    public const string MissingErrorMessageFormat = "<p class='govuk-body'>[COUNT] misisng a case reference</p>";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid case reference format</p><p class='govuk-body govuk-hint'>Format is CN12345678</p>";
    public const string TooLongErrorMessageFormat = "<p class='govuk-body'>[COUNT] too long case reference</p><p class='govuk-body govuk-hint'>Up to 12 characters are allowed</p>";
    public const string RegexFormat = "CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}";
}

public static class EmployerNameAnnotation
{
    public const string Category = "Employer";
    public const string PropertyName = "Employer name";
    public const string InvalidLengthErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid length of the employer name</p><p class='govuk-body govuk-hint'>Maximum of 99 characters allowed</p>";
    public const int MaxLength = 99;
}

public static class EmployeeArrearsOfPaymentOwedAnnotation
{
    public const string Category = "Employee";
    public const string PropertyName = "Arrears of payment owed";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid arrears of pay owed</p><p class='govuk-body govuk-hint'>Expected format is 12.34 or 100</p>";
}

public static class EmployeeNationalInsuranceNumber
{
    public const string Category = "Employee";
    public const string PropertyName = "National insurance number";
    public const string MissingErrorMessageFormat = "<p class='govuk-body'>[COUNT] missing national insurace number</p>";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid national insurace number</p><p class='govuk-body govuk-hint'>Expected format is BP011752C</p>";
    public const string RegexFormat = "[A-CEGHJ-PR-TW-Za-ceghj-pr-tw-z]{1}[A-CEGHJ-NPR-TW-Za-ceghj-npr-tw-z]{1}[0-9]{6}[A-DFMa-dfm]{1}";
}

public static class EmployeeMoneyOwedToEmployer
{
    public const string Category = "Employee";
    public const string PropertyName = "Money owed to employer";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid money owed to employer</p><p class='govuk-body govuk-hint'>Expected format is 12.34 or 100</p>";
    public const string RegexFormat = @"^\d+(\.\d{2})?$";
}

public static class EmployeeSurnameAnnotation
{
    public const string Category = "Employee";
    public const string PropertyName = "Employee surname";
    public const string MissingErrorMessageFormat = "<p class='govuk-body'>[COUNT] missing employee surname</p>";
    public const string InvalidLengthErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid length of the employee surname</p><p class='govuk-body govuk-hint'>Maximum of 99 characters allowed</p>";
    public const int MaxLength = 99;
}

public static class EmployeeBasicPayPerWeek
{
    public const string Category = "Employee pay";
    public const string PropertyName = "Basic pay per week";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid basic pay per week</p><p class='govuk-body govuk-hint'>Expected format is 12.34 or 100</p>";
    public const string RegexFormat = @"^\d+(\.\d{2})?$";
}

public static class EmployeeHolidayEntitlement
{
    public const string Category = "Employee holiday";
    public const string PropertyName = "Contracted holiday entitlement";
    public const string MissingErrorMessageFormat = "<p class='govuk-body'>[COUNT] missing contracted holiday entitlement</p>";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid contracted holiday entitlement</p><p class='govuk-body govuk-hint'>Expected format is 28.25 or 33</p>";
    public const string InvalidRangeErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid range of contracted holiday entitlement</p><p class='govuk-body govuk-hint'>0 to 365 days allowed</p>";
    public const string RegexFormat = @"^\d+(\.\d{2})?$";
    public const int MinEntitledDays = 0;
    public const int MaxEntitledDays = 365;
}

public static class EmployeeHolidayDaysCarriedForward
{
    public const string Category = "Employee holiday";
    public const string PropertyName = "Holiday days carried forward";
    public const string MissingErrorMessageFormat = "<p class='govuk-body'>[COUNT] missing holiday days carried forward</p>";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid holiday days carried forward</p><p class='govuk-body govuk-hint'>Expected format is 28.25 or 33</p>";
    public const string InvalidRangeErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid range of holiday days carried forward</p><p class='govuk-body govuk-hint'>0 to 365 days allowed</p>";
    public const string RegexFormat = @"^\d+(\.\d{2})?$";
    public const int MinEntitledDays = 0;
    public const int MaxEntitledDays = 365;
}

public static class EmployeeHolidayDaysTaken
{
    public const string Category = "Employee holiday";
    public const string PropertyName = "Holiday days taken";
    public const string MissingErrorMessageFormat = "<p class='govuk-body'>[COUNT] missing holiday days taken</p>";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid holiday days taken</p><p class='govuk-body govuk-hint'>Expected format is 28.25 or 33</p>";
    public const string InvalidRangeErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid range of holiday days taken</p><p class='govuk-body govuk-hint'>0 to 365 days allowed</p>";
    public const string RegexFormat = @"^\d+(\.\d{2})?$";
    public const int MinEntitledDays = 0;
    public const int MaxEntitledDays = 365;
}

public static class EmployeeHolidayDaysOwed
{
    public const string Category = "Employee holiday";
    public const string PropertyName = "Total number of holiday days owed";
    public const string MissingErrorMessageFormat = "<p class='govuk-body'>[COUNT] missing total nnumber of holiday days owed</p>";
    public const string InvalidErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid total nnumber of holiday days owed</p><p class='govuk-body govuk-hint'>Expected format is 28.25 or 33</p>";
    public const string InvalidRangeErrorMessageFormat = "<p class='govuk-body'>[COUNT] invalid range of total nnumber of holiday days owed</p><p class='govuk-body govuk-hint'>0 to 365 days allowed</p>";
    public const string RegexFormat = @"^\d+(\.\d{2})?$";
    public const int MinEntitledDays = 0;
    public const int MaxEntitledDays = 365;
}