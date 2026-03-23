using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Test;

public static class TestFormModels
{
    public static FormModel CreateWithYourDetailsSection()
    {
        return new FormModel
        {
            Path = "/form",
            Sections = [TestSectionModels.CreateYourDetailsSection()]
        };
    }
    
    public static FormModel CreateWithAddAnotherSection()
    {
        return new FormModel
        {
            Path = "/form",
            Sections = [TestSectionModels.CreateSectionWithAddAnother()]
        };
    }
    
    public static FormModel CreateWithIPUploadSection()
    {
        return new FormModel
        {
            Path = "/form",
            Sections = [TestSectionModels.CreateIPUploadSection()]
        };
    }
}