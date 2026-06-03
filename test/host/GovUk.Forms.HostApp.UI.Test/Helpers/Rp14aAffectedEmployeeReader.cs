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
        => ReadEmployees(XDocument.Load(filePath), employeeCount, _ => cellValue);

    public static List<AffectedEmployee> ReadAffectedEmployees(
        string filePath,
        int employeeCount,
        string[] cellValues)
        => ReadEmployees(XDocument.Load(filePath), employeeCount, i => cellValues[i % cellValues.Length]);

    public static List<AffectedEmployee> ReadAffectedEmployees(
        XDocument document,
        int employeeCount,
        string cellValue)
        => ReadEmployees(document, employeeCount, _ => cellValue);

    private static List<AffectedEmployee> ReadEmployees(
        XDocument document,
        int employeeCount,
        Func<int, string> getCellValue)
    {
        XNamespace ns = document.Root!.Name.Namespace;

        return document
            .Descendants(ns + RP14AElementNames.Employee)
            .Take(employeeCount)
            .Select((employee, index) => new AffectedEmployee
            {
                Forename = GetValue(employee, ns, RP14AElementNames.Forenames),
                Surname = GetValue(employee, ns, RP14AElementNames.Surname),
                DateOfBirth = FormatUiDate(GetValue(employee, ns, RP14AElementNames.DateOfBirth)),
                NiNumber = GetValue(employee, ns, RP14AElementNames.NINO),
                CellValue = getCellValue(index)
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
