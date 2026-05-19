namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class RP14AApiMapper : IMapper
{
    private readonly Inss.Common.IPUpload.Employee.Api.RP14A _model;
    
    public RP14AApiMapper(Inss.Common.IPUpload.Employee.Api.RP14A model)
    {
        _model = model;
    }
    
    public JsonMessage[] Map()
    {
        throw new NotImplementedException();
    }
}