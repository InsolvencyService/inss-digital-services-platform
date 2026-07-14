namespace GovUk.Forms.Domain.Test;

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
}