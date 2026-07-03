using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed class AboutYouSummaryPageLoader : SummaryPageLoader
{
    public override ValueTask LoadAsync(LoadPageContext context)
    {
        SummaryModel summary = context.CurrentPage.As<SummaryModel>();
        FullNameModel fullName = context.Section.Pages.GetFirstOf<FullNameModel>();
        AddressModel address = context.Section.Pages.GetFirstOf<AddressModel>();
        ContactDetailsModel contactDetails = context.Section.Pages.GetFirstOf<ContactDetailsModel>();
        AgeModel age = context.Section.Pages.GetFirstOf<AgeModel>();
        SalaryModel? salary = context.Section.Pages.FindFirstOf<SalaryModel>();
        BankAccountModel? bankAccount = context.Section.Pages.FindFirstOf<BankAccountModel>();
        OwnHomeModel? ownHome = context.Section.Pages.FindFirstOf<OwnHomeModel>();
        HomeValueModel? homeValue = context.Section.Pages.FindFirstOf<HomeValueModel>();

        context.Section.ReturnUrl = summary.Path;
        
        List<SummaryCategoryDetail> details = [];
        
        AppendSummaryDetail(details, "Full name", [fullName.Value], fullName.Path);
        AppendSummaryDetail(details, "Address", [address.AddressLine1], address.Path);
        AppendSummaryDetail(details, "Contact details", [contactDetails.Email.Value, contactDetails.Number.Value], contactDetails.Path);
        AppendSummaryDetail(details, "Age", [age.Value.AsString()], age.Path);

        if (salary is not null)
        {
            AppendSummaryDetail(details, "Salary", [salary.Value.AsMoney()], salary.Path);
        }

        if (bankAccount is not null)
        {
            AppendSummaryDetail(details, "Bank account", [bankAccount.AccountName, bankAccount.AccountNumber,  
                bankAccount.SortCode, bankAccount.BuildingSocietyRollNumber!], bankAccount.Path);
        }
        
        if (ownHome is not null)
        {
            AppendSummaryDetail(details, "Owns home", [ownHome.OwnsHome ? "Yes" : "No"], ownHome.Path);
            
            if (homeValue is not null)
            {
                AppendSummaryDetail(details, "Home value", [homeValue.Value.AsMoney()], ownHome.Path);
            }
        }
        
        SummaryCategory aboutYouCategory = new() { Label = "These are the details we have collected", Details = details.ToArray() };

        summary.Categories = [aboutYouCategory];
        
        return ValueTask.CompletedTask;
    }
}