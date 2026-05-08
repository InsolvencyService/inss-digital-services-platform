using GovUk.Forms.HostApp.UI.Test.Builders;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public interface IFileUploadCoordinator
{
    Task UploadFileAsync(string filePath);
    Task UploadValidRp14aAsync();
    Task UploadRp14aAsync(Rp14aScenarioBuilder scenarioBuilder);
}
