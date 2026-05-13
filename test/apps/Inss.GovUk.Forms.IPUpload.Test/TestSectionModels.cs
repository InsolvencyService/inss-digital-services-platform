using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Test;

public static class TestSectionModels
{
    public static SectionModel CreateIPUploadSection()
    {
        return new SectionModel
        {
            Path = "/form/ip-upload",
            Title = "IP Upload",
            Pages =
            [
                new IPUploadDeclarationModel { Title = "Declaration", Path = "/form/ip-upload/declaration" },
                new XmlFileUploadModel { Title = "Your Home Value", Path = "/form/ip-upload/upload" },
                new IPUploadXmlErrorsModel { Title = "IP upload errors", Path = "/form/ip-upload/errors" },
                new IPUploadXmlErrorDetailsModel {Title = "IP upload error details", Path = "/form/ip-upload/error-details"},
                new SummaryModel { Title = "Summary", Path = "/form/ip-upload/summary" },
                new PostSubmitModel{ Title = "Submitted", Path = "/form/ip-upload/submitted" }
            ]
        };
    }
}