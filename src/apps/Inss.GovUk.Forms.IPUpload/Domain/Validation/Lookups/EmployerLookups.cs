namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;

internal sealed class EmployerLookups : Dictionary<string, ErrorInfoHeader>
{
    public EmployerLookups()
    {
        this["InvalidLengthEmployerName"] = new ErrorInfoHeader
        {
            Category = "Employer", Property = "Employer name", Error = "[COUNT] invalid length of the employer name", Hint = "Maximum of 99 characters allowed"
        };
    }
}