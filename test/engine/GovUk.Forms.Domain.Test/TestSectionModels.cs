namespace GovUk.Forms.Domain.Test;

public static class TestSectionModels
{
    public static SectionModel CreateSectionWithAddAnother()
    {
        return new SectionModel
        {
            Path = "/form/add-another-section",
            Pages =
            [
                new AddAnotherGroup
                {
                    Pages =
                    [
                        new FullNameModel
                        {
                            Title = "Fullname", Path = "/form/add-another-section/fullname", MetaData = { Group = "Group1" }
                        },
                        new AgeModel
                        {
                            Title = "Age", Path = "/form/add-another-section/age", MetaData = { Group = "Group1" }
                        },
                        new CheckAnswersModel
                        {
                            Title = "Check Answers", Path = "/form/add-another-section/check-answers", MetaData = { Group = "Group1" }
                        },
                        new AddAnotherModel
                        {
                            Title = "Add Another", Path = "/form/add-another-section/add-another", MetaData = { Group = "Group1" }
                        }
                    ],
                    MetaData =
                    {
                        Group = "Group1"
                    }
                },
                new SummaryModel { Title = "Summary", Path = "/form/add-another-section/summary" }
            ]
        };
    }
    
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
    
    public static SectionModel CreateAssetSection()
    {
        return new SectionModel
        {
            Path = "/form/your-assets",
            Title = "Your Assets",
            Pages = [
                new MoneyModel { Title = "Your Salary", Path = "/form/your-assets/your-salary" },
                new MoneyModel { Title = "Your Savings", Path = "/form/your-assets/your-savings" },
                new MoneyModel { Title = "Your Mortgage", Path = "/form/your-assets/your-mortgage" },
                new SummaryModel { Title = "Summary", Path = "/form/your-assets/summary" }
            ]
        };
    }
}