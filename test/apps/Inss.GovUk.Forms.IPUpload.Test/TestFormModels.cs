using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Test;

public static class TestFormModels
{
    public static FormModel CreateWithIPUploadSection()
    {
        return new FormModel
        {
            Path = "/form",
            Sections = [TestSectionModels.CreateIPUploadSection()]
        };
    }
}