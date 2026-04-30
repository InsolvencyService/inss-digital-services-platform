using GovUk.Forms.HostApp.UI.Test.Coordinators;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public sealed class ComonSteps
{
    private readonly CommonCoordinator _commonCoordinator;

    public ComonSteps(CommonCoordinator commonCoordinator)
    {
        _commonCoordinator = commonCoordinator;
    }

    [Given("I am on the upload page")]
    public async Task GivenIAmOnTheUploadPage()
    {
        await _commonCoordinator.VerifyThatUploadDocumentPageIsDisplayedAsync();
    }

}
