using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Test;

public static class TestSectionDefaults
{
    public static void YourDetails(SectionModel section)
    {
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        fullName.SetCompleted();

        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        address.AddressLine1 = "101 Ivy Terrace";
        address.AddressLine2 = "Wood Lane";
        address.TownCity = "Treetown";
        address.County = "Oak County";
        address.Postcode = "TN33 0DN";
        address.SetCompleted();

        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        age.Value = 44;
        age.SetCompleted();

        SalaryModel salary = section.Pages.GetFirstOf<SalaryModel>();
        salary.Value = 50_000;
        salary.SetCompleted();

        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        bankAccount.SortCode = "11-22-33";
        bankAccount.AccountNumber = "12345678";
        bankAccount.SetCompleted();
    }
}