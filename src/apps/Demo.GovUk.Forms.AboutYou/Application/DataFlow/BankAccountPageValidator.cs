using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.PageFlow;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed partial class BankAccountPageValidator : DefaultPageValidator
{
    private static readonly Regex _regex = BuildingSocietyRollNumberRegex();
    
    public override async ValueTask ValidateAsync(ValidatePageContext context)
    {
        await base.ValidateAsync(context);
        BankAccountModel bankAccount = context.CurrentPage.As<BankAccountModel>();

        if (bankAccount is { AccountNumber: "12345678", SortCode: "11-22-33" })
        {
            context.ValidationResults.AddResult(
                "The bank account details are invalid", [nameof(bankAccount.AccountNumber), nameof(bankAccount.SortCode)]);
        }

        if (!string.IsNullOrWhiteSpace(bankAccount.BuildingSocietyRollNumber))
        {
            if (bankAccount.BuildingSocietyRollNumber.Length is < 1 or > 18)
            {
                context.ValidationResults.AddResult(
                    "Building society roll number must be between 1 and 18 characters", [nameof(bankAccount.BuildingSocietyRollNumber)]);
            }
            
            if (!_regex.IsMatch(bankAccount.BuildingSocietyRollNumber))
            {
                context.ValidationResults.AddResult(
                    "Building society roll number must only include letters a to z, numbers, hyphens, spaces, forward slashes and full stops", 
                    [nameof(bankAccount.BuildingSocietyRollNumber)]);
            }
        }
    }
    
    [GeneratedRegex(@"^[A-Za-z0-9\-\/\. ]+$", RegexOptions.Compiled)]
    private static partial Regex BuildingSocietyRollNumberRegex();
}