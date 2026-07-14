using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Test;

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
    
    public static FormModel CreateWithYourAssetsSection()
    {
        return new FormModel
        {
            Path = "/form",
            Sections = [TestSectionModels.CreateYourAssetsSection()]
        };
    }
}