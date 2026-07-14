using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Types;

namespace Demo.GovUk.Forms.AboutYou.Domain;

public sealed class ContactDetailsModel : PageModel
{
    public EmailAddress Email { get; init; } = new() { Label = "What is your email address?", LabelSize = LabelSizes.Large };

    public TelephoneNumber Number { get; init; } = new() { Label = "What is your home telephone number?", LabelSize = LabelSizes.Large };
    
    public override string[] GetSummaryInfo()
    {
        return [Email.Value, Number.Value];
    }
    
    public override void CopyTo(PageModel target)
    {
        ContactDetailsModel contactDetails = target.As<ContactDetailsModel>();
        contactDetails.Email.Value = Email.Value;
        contactDetails.Number.Value = Number.Value;
    }

    public override void ClearValues()
    {
        base.ClearValues();
        Email.Value = string.Empty;
        Number.Value = string.Empty;
    }
}