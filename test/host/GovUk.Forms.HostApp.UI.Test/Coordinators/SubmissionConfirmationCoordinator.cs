using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Submit;
using Notify.Models;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public sealed class SubmissionConfirmationCoordinator(
    ISubmissionConfirmationPage submissionConfirmation,
    ICosmosDbService cosmosDbService,
    INotifyService notifyService,
    IPlaywrightDriver playwrightDriver,
    ScenarioContext scenarioContext,
    TestArtifacts testArtifacts)
    : BaseCoordinator(testArtifacts)
{
    private const string UserTypeKey = "UserType";
    private const string DeliveredStatus = "delivered";
    private const string EmailType = "email";

    private const int NotificationPollingAttempts = 24;
    private static readonly TimeSpan _notificationPollingDelay = TimeSpan.FromSeconds(5);

    private Notification? _submissionConfirmationEmail;
    private string _submissionReference = string.Empty;

    public async Task VerifySubmissionConfirmationPageIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Submission Confirmation page is displayed",
            async () =>
            {
                await submissionConfirmation.WaitForPageToLoadAsync();

                AddAllureLog(
                    "SubmissionConfirmation",
                    "Submission Confirmation page displayed successfully");
            });
    }

    public async Task UploadAnotherFormAsync()
    {
        await ExecuteStepAsync(
            "Upload another form",
            async () =>
            {
                await submissionConfirmation.WaitForPageToLoadAsync();

                await submissionConfirmation.ClickUploadAnotherFormButtonAsync();

                AddAllureLog(
                    "UploadAnotherForm",
                    "Upload another form button clicked successfully");
            });
    }

    public async Task RetrieveSubmissionConfirmationEmailAsync()
    {
        await ExecuteStepAsync(
            "Retrieve submission confirmation email",
            async () =>
            {
                _submissionReference =
                    await submissionConfirmation.GetReferenceNumberAsync();

                string? notificationId = null;

                for (int attempt = 1; attempt <= NotificationPollingAttempts; attempt++)
                {
                    notificationId =
                        await cosmosDbService.GetIpEmailReceiptAsync(
                            _submissionReference);

                    if (!string.IsNullOrWhiteSpace(notificationId))
                    {
                        break;
                    }

                    AddAllureLog(
                        "NotificationPolling",
                        $"Reference={_submissionReference}; " +
                        $"Attempt={attempt}/{NotificationPollingAttempts}");

                    if (attempt < NotificationPollingAttempts)
                    {
                        await playwrightDriver.Page.WaitForTimeoutAsync((float)(_notificationPollingDelay.TotalSeconds * 1000));
                    }
                }

                Assert.That(
                    notificationId,
                    Is.Not.Null.And.Not.Empty,
                    $"No email receipt found in Cosmos DB for reference '{_submissionReference}' after {(int)(NotificationPollingAttempts * _notificationPollingDelay.TotalSeconds)} seconds.");

                _submissionConfirmationEmail =
                    await notifyService.GetNotificationByIdAsync(notificationId!);

                Assert.That(
                    _submissionConfirmationEmail,
                    Is.Not.Null,
                    $"No notification found in GOV.UK Notify for ID '{notificationId}'.");

                AddAllureLog(
                    "SubmissionConfirmationEmail",
                    $"Reference='{_submissionReference}', NotificationId='{notificationId}'");
            });
    }

    public Task VerifySubmissionConfirmationEmailContainsRP14ADetailsAsync() =>
        VerifySubmissionConfirmationEmailAsync("RP14A");

    public Task VerifySubmissionConfirmationEmailContainsRP14DetailsAsync() =>
        VerifySubmissionConfirmationEmailAsync("RP14");

    private async Task VerifySubmissionConfirmationEmailAsync(string formType)
    {
        await ExecuteStepAsync(
            $"Verify submission confirmation email contains {formType} details",
            () =>
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(
                        _submissionConfirmationEmail,
                        Is.Not.Null,
                        "Submission confirmation email has not been retrieved.");

                    Assert.That(
                        _submissionReference,
                        Is.Not.Empty,
                        "Submission reference has not been captured.");

                    Notification email = _submissionConfirmationEmail!;

                    string expectedSubject =
                        $"Update on your {formType} submission – reference {_submissionReference}";

                    Assert.That(
                        email.status,
                        Is.EqualTo(DeliveredStatus),
                        $"Expected status '{DeliveredStatus}' but was '{email.status}'.");

                    Assert.That(
                        email.type,
                        Is.EqualTo(EmailType),
                        $"Expected type '{EmailType}' but was '{email.type}'.");

                    Assert.That(
                        email.subject,
                        Is.EqualTo(expectedSubject),
                        "Email subject does not match expected value.");

                    string? expectedRecipient = GetExpectedRecipient();

                    if (!string.IsNullOrWhiteSpace(expectedRecipient))
                    {
                        Assert.That(
                            email.emailAddress,
                            Is.EqualTo(expectedRecipient),
                            "Email recipient is incorrect.");
                    }

                    Assert.That(
                        email.sentAt,
                        Is.Not.Null,
                        "Email sent date/time was not populated.");

                    foreach (string expectedContent in GetExpectedBodyContent(
                                 formType,
                                 _submissionReference))
                    {
                        Assert.That(
                            email.body,
                            Does.Contain(expectedContent),
                            $"Email body is missing '{expectedContent}'.");
                    }
                }

                AddAllureLog(
                    "SubmissionConfirmationEmailVerification",
                    $"FormType='{formType}', " +
                    $"Reference='{_submissionReference}', " +
                    $"Status='{_submissionConfirmationEmail?.status}', " +
                    $"Recipient='{_submissionConfirmationEmail?.emailAddress}'");

                return Task.CompletedTask;
            });
    }

    private string? GetExpectedRecipient()
    {
        if (!scenarioContext.TryGetValue(UserTypeKey, out string? userType) ||
            string.IsNullOrWhiteSpace(userType))
        {
            return null;
        }

        return UserFactory.GetUser(userType).Email;
    }

    private static IEnumerable<string> GetExpectedBodyContent(string formType, string submissionReference)
    {
        yield return $"Your {formType} submission has been processed.";
        yield return "Status: succeeded";
        yield return "If your submission was successful, no further action is needed.";
        yield return "If your submission failed, you will need to check your file and upload it again.";
        yield return "Submission details";
        yield return $"Reference number: {submissionReference}";
        yield return "Uploaded on:";
        yield return "If you need assistance contact the helpdesk on";
        yield return "RPS.Stakeholder@insolvency.gov.uk";
    }
}