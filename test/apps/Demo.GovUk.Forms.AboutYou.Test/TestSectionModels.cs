using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Test;

public static class TestSectionModels
{
    public static SectionModel CreateYourDetailsSection()
    {
        return new SectionModel
        {
            Path = "/form/your-details",
            Title = "Your Details",
            Pages = [
                new FullNameModel { Title = "Your Name", Path = "/form/your-details/your-fullname" },
                new AddressModel { Title = "Your Address", Path = "/form/your-details/your-address" },
                new AgeModel { Title = "Your Age", Path = "/form/your-details/your-age" },
                new SalaryModel { Title = "Your Salary", Path = "/form/your-details/add-another" },
                new BankAccountModel { Title = "Your Bank Account", Path = "/form/your-details/your-bank-account" },
                new SummaryModel { Title = "Summary", Path = "/form/your-details/summary" }
            ]
        };
    }
    
    public static SectionModel CreateYourAssetsSection()
    {
        return new SectionModel
        {
            Path = "/form/your-assets",
            Title = "Your Assets",
            Pages = [
                new OwnHomeModel { Title = "Own Your Home", Path = "/form/your-assets/own-home" },
                new HomeValueModel { Title = "Your Home Value", Path = "/form/your-assets/home-value" },
                new SummaryModel { Title = "Summary", Path = "/form/your-details/summary" }
            ]
        };
    }
}