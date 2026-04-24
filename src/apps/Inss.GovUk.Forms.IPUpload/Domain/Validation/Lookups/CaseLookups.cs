namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;

internal sealed class CaseLookups : Dictionary<string, ErrorInfoHeader>
{
    internal CaseLookups()
    {
        this["UnknownCaseReference"] = new ErrorInfoHeader
        {
            Category = "Case", Property = "Case reference", Error = "[COUNT] case reference have not been matched in our system"
        };
        this["MissingCaseReference"] = new ErrorInfoHeader
        {
            Category = "Case", Property = "Case reference", Error = "[COUNT] missing a case reference"
        };
        this["InvalidFormatCaseReference"] = new ErrorInfoHeader
        {
            Category = "Case", Property = "Case reference", Error = "[COUNT] invalid case reference format", Hint = "Format is CN12345678"
        };
        this["InvalidLengthCaseReference"] = new ErrorInfoHeader
        {
            Category = "Case", Property = "Case reference", Error = "[COUNT] too long case reference", Hint = "Up to 12 characters are allowed"
        };
    }
}