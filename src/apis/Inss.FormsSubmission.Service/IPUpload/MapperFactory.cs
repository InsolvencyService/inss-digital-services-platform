using Inss.FormsSubmission.Service.IPUpload.Employee;
using Inss.FormsSubmission.Service.IPUpload.Employer;
using Inss.FormsSubmission.Service.IPUpload.Exceptions;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class MapperFactory : IMapperFactory
{
    public IMapper Create(object model)
    {
        return model switch
        {
            Inss.Common.IPUpload.Employee.Spreadsheet.RP14A x => new RP14ASpreadsheetMapper(x),
            Inss.Common.IPUpload.Employee.Api.RP14A x => new RP14AApiMapper(x),
            Inss.Common.IPUpload.Employer.Spreadsheet.RP14 x => new RP14SpreadsheetMapper(x),
            Inss.Common.IPUpload.Employer.Api.RP14 x => new RP14ApiMapper(x),
            _ => throw new IPUploadMappingException($"Unable to map the model {model.GetType()} to an RP14(A) type.")
        };
    }
}