using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public static class Rp14aAffectedEmployeeReader
{
    public static List<AffectedEmployee> ReadAffectedEmployees(
        string filePath,
        int employeeCount,
        string cellValue)
    {
        XDocument document = XDocument.Load(filePath);
        XNamespace ns = document.Root!.Name.Namespace;

        return document
            .Descendants(ns + RP14AElementNames.Employee)
            .Take(employeeCount)
            .Select(employee => new AffectedEmployee
            {
                Forename = GetValue(employee, ns, RP14AElementNames.Forenames),
                Surname = GetValue(employee, ns, RP14AElementNames.Surname),
                DateOfBirth = FormatUiDate(GetValue(employee, ns, RP14AElementNames.DateOfBirth)),
                NiNumber = GetValue(employee, ns, RP14AElementNames.NationalInsuranceNumber),
                CellValue = cellValue
            })
            .ToList();
    }

    public static List<AffectedEmployee> ReadAffectedEmployees(
    string filePath,
    int employeeCount,
    string[] cellValues)
    {
        XDocument document = XDocument.Load(filePath);
        XNamespace ns = document.Root!.Name.Namespace;

        return document
            .Descendants(ns + RP14AElementNames.Employee)
            .Take(employeeCount)
            .Select((employee, index) => new AffectedEmployee
            {
                Forename = GetValue(employee, ns, RP14AElementNames.Forenames),
                Surname = GetValue(employee, ns, RP14AElementNames.Surname),
                DateOfBirth = FormatUiDate(GetValue(employee, ns, RP14AElementNames.DateOfBirth)),
                NiNumber = GetValue(employee, ns, RP14AElementNames.NationalInsuranceNumber),
                CellValue = cellValues[index % cellValues.Length]
            })
            .ToList();
    }

    private static string GetValue(
        XElement parent,
        XNamespace ns,
        string elementName)
    {
        return parent
            .Descendants(ns + elementName)
            .FirstOrDefault()
            ?.Value ?? string.Empty;
    }

    private static string FormatUiDate(string date)
    {
        if (string.IsNullOrWhiteSpace(date))
        {
            return string.Empty;
        }

        return DateTime
            .ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            .ToString("M/d/yyyy", CultureInfo.InvariantCulture);
    }
}
