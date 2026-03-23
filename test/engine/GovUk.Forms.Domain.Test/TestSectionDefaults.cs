using GovUk.Forms.Domain.Enums;

namespace GovUk.Forms.Domain.Test;

public static class TestSectionDefaults
{
    public static void YourDetails(SectionModel section)
    {
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        fullName.EditMode = PageEditTypes.Saved;

        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        address.AddressLine1 = "101 Ivy Terrace";
        address.AddressLine2 = "Wood Lane";
        address.TownCity = "Treetown";
        address.County = "Oak County";
        address.Postcode = "TN33 0DN";
        address.EditMode = PageEditTypes.Saved;

        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        age.Value = 44;
        age.EditMode = PageEditTypes.Saved;

        SalaryModel salary = section.Pages.GetFirstOf<SalaryModel>();
        salary.Value = 50_000;
        salary.EditMode = PageEditTypes.Saved;

        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        bankAccount.SortCode = "11-22-33";
        bankAccount.AccountNumber = "12345678";
        bankAccount.EditMode = PageEditTypes.Saved;
    }
}