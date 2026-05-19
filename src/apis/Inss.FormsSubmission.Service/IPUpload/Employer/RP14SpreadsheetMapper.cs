namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public sealed class RP14SpreadsheetMapper : IMapper
{
    private readonly Inss.Common.IPUpload.Employer.Spreadsheet.RP14 _model;
    
    public RP14SpreadsheetMapper(Inss.Common.IPUpload.Employer.Spreadsheet.RP14 model)
    {
        _model = model;
    }
    
    public JsonMessage[] Map()
    {
        throw new NotImplementedException();
    }
}

public sealed class RP14ApiMapper : IMapper
{
    private readonly Inss.Common.IPUpload.Employer.Api.RP14 _model;
    
    public RP14ApiMapper(Inss.Common.IPUpload.Employer.Api.RP14 model)
    {
        _model = model;
    }
    
    public JsonMessage[] Map()
    {
        throw new NotImplementedException();
    }
}