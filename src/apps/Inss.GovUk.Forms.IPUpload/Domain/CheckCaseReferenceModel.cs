using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Types;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class CheckCaseReferenceModel : PageModel
{
    public SingleLineText CaseReference { get; set; } = new()
    {
        Label = "Whats the case reference number?",
        Hint = "For example, �CN12345678�. This must match the case reference number in your form.",
        LabelSize = LabelSizes.Large
    };
    
    public override void CopyTo(PageModel target)
    {
        CheckCaseReferenceModel checkCaseReference = target.As<CheckCaseReferenceModel>();
        checkCaseReference.CaseReference.Value = checkCaseReference.CaseReference.Value;
    }
}