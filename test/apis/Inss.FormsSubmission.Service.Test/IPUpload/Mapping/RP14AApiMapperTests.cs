using System.Text.Json;
using Inss.Common.IPUpload.Employee.Api;
using Inss.FormsSubmission.Service.IPUpload;
using Inss.FormsSubmission.Service.IPUpload.Employee;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Xunit;

namespace Inss.FormsSubmission.Service.Test.IPUpload.Mapping;

public sealed class RP14AApiMapperTests
{
    [Fact]
    public void SparseModel_Map_DoesNotThrowAndUsesNullGuardFallbacks()
    {
        RP14A model = BuildSparseModel();
        RP14AApiMapper sut = new(model);

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

    private static RP14A BuildSparseModel()
    {
        return new RP14A
        {
            Header = new RP14AHeader { CaseReference = "CN10000000" },
            Employee =
            [
                new RP14AEmployee
                {
                    EmployeeName = null,
                    NINO = null,
                    PayDetails = null,
                    Holiday = null
                }
            ]
        };
    }
}
