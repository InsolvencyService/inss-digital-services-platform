using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Models.TestData;

public static class TestFactory
{
    public static AffectedEmployee CreateAffectedEmployee(
        string? surname = null,
        string? forename = null,
        string? niNumber = null,
        string? cellValue = null)
    {
        return new AffectedEmployee
        {
            Surname = surname ?? ScenarioConstant.Surname,
            Forename = forename ?? ScenarioConstant.Forename,
            DateOfBirth = UiDateOfBirth(),
            NiNumber = niNumber ?? ScenarioConstant.NationalInsuranceNumber,
            CellValue = cellValue ?? string.Empty
        };
    }

    public static UploadErrorSummary CreateErrorSummary(
        string type,
        string message,
        string? hint = null,
        string category = "")
    {
        return new UploadErrorSummary(
            Category: category,
            ErrorType: type,
            ErrorMessage: message,
            HintText: hint);
    }


    public static string UiDateOfBirth() =>
        FormatUiDate(ScenarioConstant.DOB);


    private static string FormatUiDate(string value) =>
        DateTime
            .ParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture)
            .ToString(
                "d/M/yyyy",
                CultureInfo.InvariantCulture);
}
