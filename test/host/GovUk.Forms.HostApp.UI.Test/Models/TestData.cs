using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Models;

public partial class TestData
{
    public class Errors
    {
        public string Message { get; set; } = string.Empty;
        public string Hint { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }


    public static AffectedEmployee CreateAffectedEmployee(
        string? surname = null,
        string? forename = null,
        string? niNumber = null,
        string? cellValue = null)
    {
        return new AffectedEmployee
        {
            Surname = surname ?? ScenarioConstant.Surname,
            Forename = forename ?? ScenarioConstant.Forname,
            DateOfBirth = UiDateOfBirth(),
            NiNumber = niNumber ?? ScenarioConstant.NationalInsuranceNumber,
            CellValue = cellValue ?? string.Empty
        };
    }

    public static UploadErrorSummary CreateEmployeeErrorSummary(
        string errorType,
        string errorMessage,
        string? hintText = null)
    {
        return new UploadErrorSummary(
            Category: "Employee",
            ErrorType: errorType,
            ErrorMessage: errorMessage,
            HintText: hintText);
    }

    public static string UiDateOfBirth() =>
     DateTime
    .ParseExact(ScenarioConstant.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture)
    .ToString("d/M/yyyy", CultureInfo.InvariantCulture);

    public static string FormatDateRange(string start, string end)
    {
        return $"{DateTime.Parse(start, CultureInfo.InvariantCulture):M/d/yyyy}, {DateTime.Parse(end, CultureInfo.InvariantCulture):M/d/yyyy}";
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

}
