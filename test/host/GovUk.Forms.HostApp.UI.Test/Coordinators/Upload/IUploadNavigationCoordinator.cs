namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

/// <summary>
/// Coordinator for navigation operations (continue, back, next page).
/// </summary>
public interface IUploadNavigationCoordinator
{
    Task ClickOnContinueButtonAsync();
    Task ClickOnBackButtonAsync();
    Task NavigateToFeedbackPageAsync();
    Task NavigateToSubmitPageAsync();
}
