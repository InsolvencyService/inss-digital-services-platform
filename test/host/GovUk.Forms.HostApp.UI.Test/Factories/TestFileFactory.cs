using GovUk.Forms.HostApp.UI.Test.Helpers;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GovUk.Forms.HostApp.UI.Test.Factories;

public static class TestFileFactory
{
    private const int OneMb = 1024 * 1024;
    private const string ClosingTag = "</ns1:RP14A>";
    private const string NsPrefix = "ns1";
    private const string Namespace = "http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application";

    private const string PaddingComment =
        "\n<!-- Padding Padding Padding Padding Padding -->";

    private static readonly XmlSerializer _serializer = new(typeof(RP14A));
    private static readonly XmlSerializerNamespaces _serializerNamespaces = CreateSerializerNamespaces();

    public static string CreateValidXmlFileAboveSize(
        TestArtifacts artifacts,
        int maxSizeMb)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        return BuildPaddedXmlFile(
            artifacts,
            $"rp14a-above-{maxSizeMb}mb.xml",
            (maxSizeMb * OneMb) + 1);
    }

    public static string CreateFile(
        TestArtifacts artifacts,
        string fileName,
        string content)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        string filePath = artifacts.FilePath(fileName);

        File.WriteAllText(filePath, content, Encoding.UTF8);

        return filePath;
    }

    public static string CreateEmptyFile(TestArtifacts artifacts)
    {
        return CreateFile(artifacts, "empty-rp14a.xml", string.Empty);
    }

    public static string CreateUnsupportedFile(
        TestArtifacts artifacts,
        string extension)
    {
        return CreateFile(
            artifacts,
            $"unsupported-file{extension}",
            "This is not a valid RP14A XML file.");
    }

    public static string CreateXmlWithWrongContent(
        TestArtifacts artifacts)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        string filePath = artifacts.FilePath(
            "invalid-rp14a-structure.xml");

        File.WriteAllText(
            filePath,
            """
            <InvalidRP14A>
              <Employee>
                <Header>
                  <CaseReference>CN12445678</CaseReference>
                </Header>
                <EmployerName>Employer Test</EmployerName>
              </Employee>
            </InvalidRP14A>
            """,
            Encoding.UTF8);

        return filePath;
    }

    private static string BuildPaddedXmlFile(
        TestArtifacts artifacts,
        string fileName,
        int targetBytes)
    {
        string filePath = artifacts.FilePath(fileName);

        string xml = BuildDefaultXmlContent();

        int closingTagStart = xml.LastIndexOf(ClosingTag, StringComparison.Ordinal);
        string xmlBody = xml[..closingTagStart];
        string xmlClose = xml[closingTagStart..];

        int bodyBytes = Encoding.UTF8.GetByteCount(xmlBody);
        int closeBytes = Encoding.UTF8.GetByteCount(xmlClose);
        int paddingBytes = Encoding.UTF8.GetByteCount(PaddingComment);
        int paddingCount = Math.Max(0, (targetBytes - bodyBytes - closeBytes + paddingBytes - 1) / paddingBytes);

        StringBuilder builder = new(bodyBytes + (paddingCount * PaddingComment.Length) + closeBytes);
        builder.Append(xmlBody);
        for (int i = 0; i < paddingCount; i++)
        {
            builder.Append(PaddingComment);
        }
        builder.Append(xmlClose);

        File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);

        return filePath;
    }

    private static string BuildDefaultXmlContent()
    {
        RP14A model = new()
        {
            Employee =
            [
                new RP14AEmployee
                {
                    Header = new RP14AEmployeeHeader { CaseReference = "CN00345678" },
                    EmployerName = "Employer Test",
                    EmployeeName = new NameType { Surname = "Surname Test", Forenames = "Forname Test", Title = "Mr" },
                    NIClass = "A",
                    NINO = "BP011752C",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    DateOfBirthSpecified = true,
                    StartDate = new DateTime(2000, 1, 1),
                    StartDateSpecified = true,
                    DateNoticeGiven = new DateTime(2020, 1, 1),
                    DateNoticeGivenSpecified = true,
                    EndDate = new DateTime(2020, 3, 1),
                    EndDateSpecified = true,
                    IsDirector = YesNoType.No,
                    IsDirectorSpecified = true,
                    AverageHoursWorked = 37m,
                    AverageHoursWorkedSpecified = true,
                    MoneyOwedToEmployer = 5000m,
                    MoneyOwedToEmployerSpecified = true,
                    EntitledToRedundancyPay = YesNoType.Yes,
                    EntitledToRedundancyPaySpecified = true,
                    EntitledToNoticePay = YesNoType.Yes,
                    EntitledToNoticePaySpecified = true,
                    PayDetails = new RP14AEmployeePayDetails
                    {
                        BasicPayPerWeek = 1000m,
                        BasicPayPerWeekSpecified = true,
                        WeeklyPayDay = RP14AEmployeePayDetailsWeeklyPayDay.Monday,
                        WeeklyPayDaySpecified = true,
                        ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                        {
                            ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1
                            {
                                AOP1StartDate = new DateTime(2020, 1, 10),
                                AOP1StartDateSpecified = true,
                                AOP1EndDate = new DateTime(2020, 1, 11),
                                AOP1EndDateSpecified = true,
                                AOPOwed1 = 100m,
                                AOPOwed1Specified = true,
                                AOPPayType1 = RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1AOPPayType1.wages,
                                AOPPayType1Specified = true
                            },
                            ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                            ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                            ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                        }
                    },
                    Holiday = new RP14AEmployeeHoliday
                    {
                        HolidayYearStart = new DateTime(2020, 1, 1),
                        HolidayYearStartSpecified = true,
                        HolidayContractedEntitlementDays = 30m,
                        HolidayContractedEntitlementDaysSpecified = true,
                        HolidayDaysCarriedForward = 10m,
                        HolidayDaysCarriedForwardSpecified = true,
                        HolidayDaysTaken = 2m,
                        HolidayDaysTakenSpecified = true,
                        NoDaysHolidayOwed = 10m,
                        NoDaysHolidayOwedSpecified = true
                    }
                }
            ]
        };

        XmlWriterSettings settings = new()
        {
            Indent = true,
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
        };

        using StringWriter sw = new();
        using XmlWriter writer = XmlWriter.Create(sw, settings);
        _serializer.Serialize(writer, model, _serializerNamespaces);

        return sw.ToString();
    }

    private static XmlSerializerNamespaces CreateSerializerNamespaces()
    {
        XmlSerializerNamespaces ns = new();
        ns.Add(NsPrefix, Namespace);
        return ns;
    }
}
