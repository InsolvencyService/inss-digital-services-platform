using Inss.FormsSubmission.Service.IPUpload.Exceptions;
using Inss.FormsSubmission.Service.IPUpload.Mapping;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class MapperFactory : IMapperFactory
{
    public IMapper Create(object model)
    {
        return model switch
        {
            Inss.Common.IPUpload.Employee.Spreadsheet.RP14A employeeSpreadsheet => new RP14ASpreadsheetMapper(employeeSpreadsheet),
            Inss.Common.IPUpload.Employee.Api.RP14A employeeApi => new RP14AApiMapper(employeeApi),
            Inss.Common.IPUpload.Employer.Spreadsheet.RP14 employerSpreadsheet => new RP14SpreadsheetMapper(employerSpreadsheet),
            Inss.Common.IPUpload.Employer.Api.RP14 employerApi => new RP14ApiMapper(employerApi),
            _ => throw new IPUploadMappingException($"Unable to map the model {model.GetType()} to an RP14(A) type.")
        };
    }
}