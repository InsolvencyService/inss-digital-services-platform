using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Test;

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
                    MetaData = { Group = "TestGroup" },
                    Pages =
                    [
                        new FullNameModel { Title = "Fullname", Path = "/form/add-another-section/fullname", MetaData = { Group = "TestGroup" } },
                        new AgeModel { Title = "Age", Path = "/form/add-another-section/age", MetaData = { Group = "TestGroup" } },
                        new CheckAnswersModel { Title = "Check Answers", Path = "/form/add-another-section/check-answers", MetaData = { Group = "TestGroup" } },
                        new RemoveModel { Title = "Remove Item", Path = "/form/add-another-section/remove-item", MetaData = { Group = "TestGroup" } },
                        new AddAnotherModel { Title = "Add Another", Path = "/form/add-another-section/add-another", MetaData = { Group = "TestGroup" } }
                    ]
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
    
    public static SectionModel CreateIPUploadSection()
    {
        return new SectionModel
        {
            Path = "/form/ip-upload",
            Title = "IP Upload",
            Pages = [
                new StaticHtmlModel { Title = "Declaration", Path = "/form/ip-upload/declaration", Key = "Declaration" },
                new FileUploadModel { Title = "Upload File", Path = "/form/ip-upload/upload" },
                new SummaryModel { Title = "Summary", Path = "/form/ip-upload/summary" }
            ]
        };
    }
}