using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

/// <summary>
/// Coordinator for upload verification and assertions.
/// </summary>
public interface IUploadVerificationCoordinator
{
    Task VerifyThatFileIsUploadedAsync();
    Task VerifyOnlyOneFileUploadedAsync();
    Task VerifyInvalidFileExtensionErrorAsync(UploadFileError uploadFileError);
}
