using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Features.Submission;

// Reqnroll regenerates SubmissionConfirmation.feature.cs (gitignored) on every build,
// so retry is applied here via the partial class instead of editing the generated file.
// Scenarios in this feature depend on a live Dynamics submission whose success/failure
// is reported asynchronously by email; that call can fail transiently in the shared
// Dev environment even though the app itself behaved correctly. NUnit's built-in
// RetryAttribute is method-only, so FixtureRetryAttribute reruns the whole scenario
// (fresh upload/submit/reference) at the class level instead.
[FixtureRetry(2)]
public partial class SubmissionConfirmationFeature
{
}
