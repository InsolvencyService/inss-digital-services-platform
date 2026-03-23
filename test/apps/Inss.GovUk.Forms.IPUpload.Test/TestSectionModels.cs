using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Test;

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
    
    public static SectionModel CreateIPUploadSection()
    {
        return new SectionModel
        {
            Path = "/form/ip-upload",
            Title = "IP Upload",
            Pages =
            [
                new IPUploadDeclarationModel { Title = "Declaration", Path = "/form/ip-upload/declaration", Key = "Form" },
                new XmlFileUploadModel { Title = "Your Home Value", Path = "/form/ip-upload/upload" },
                new IPUploadXmlErrorsModel { Title = "IP upload errors", Path = "/form/ip-upload/errors" },
                new SummaryModel { Title = "Summary", Path = "/form/ip-upload/summary" }
            ]
        };
    }
}