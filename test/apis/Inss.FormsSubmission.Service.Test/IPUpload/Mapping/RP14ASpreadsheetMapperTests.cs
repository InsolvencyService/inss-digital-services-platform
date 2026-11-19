using System.Text.Json;
using Inss.FormsSubmission.Service.IPUpload;
using Inss.FormsSubmission.Service.IPUpload.Employee;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Xunit;
using RP14ASpreadsheet = Inss.Common.IPUpload.Employee.Spreadsheet.RP14A;
using RP14ASpreadsheetEmployee = Inss.Common.IPUpload.Employee.Spreadsheet.RP14AEmployee;
using RP14ASpreadsheetEmployeeHeader = Inss.Common.IPUpload.Employee.Spreadsheet.RP14AEmployeeHeader;

namespace Inss.FormsSubmission.Service.Test.IPUpload.Mapping;

public sealed class RP14ASpreadsheetMapperTests
{
    [Fact]
    public void SparseModel_Map_DoesNotThrowAndUsesNullGuardFallbacks()
    {
        RP14ASpreadsheet model = BuildSparseModel();
        RP14ASpreadsheetMapper sut = new(model);

        Exception? exception = Record.Exception(sut.Map);
        Assert.Null(exception);

        JsonMessage[] result = sut.Map();
        Assert.NotEmpty(result);

        EmployeeInformation? mapped = JsonSerializer.Deserialize<EmployeeInformation>(result[0].Json);

        Assert.NotNull(mapped);

        EmployeeInformation mappedValue = mapped;

        Assert.Null(mappedValue.EmployerName);
        Assert.Null(mappedValue.Employee.Title);
        Assert.Null(mappedValue.Employee.FirstNames);
        Assert.Null(mappedValue.Employee.LastName);
        Assert.Null(mappedValue.Employee.NationalInsuranceNumber);
        Assert.Null(mappedValue.Employee.Pay);
    }

    private static RP14ASpreadsheet BuildSparseModel()
    {
        return new RP14ASpreadsheet
        {
            Employee =
            [
                new RP14ASpreadsheetEmployee
                {
                    Header = new RP14ASpreadsheetEmployeeHeader { CaseReference = "CN10000000" },
                    EmployeeName = null,
                    NINO = null,
                    PayDetails = null,
                    Holiday = null
                }
            ]
        };
    }
}
