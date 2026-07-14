using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace GovUk.Forms.HostApp.UI.Test.Support;

// NUnit's built-in RetryAttribute only targets Method, so it cannot be applied to a
// Reqnroll-generated TestFixture via a partial class. This reproduces the same
// retry-on-failure behaviour at the class level using NUnit's command-wrapping extensibility.
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FixtureRetryAttribute(int tryCount) : NUnitAttribute, IWrapTestMethod
{
    public TestCommand Wrap(TestCommand command) => new RetryCommand(command, tryCount);

    private sealed class RetryCommand(TestCommand innerCommand, int tryCount)
        : DelegatingTestCommand(innerCommand)
    {
        public override TestResult Execute(TestExecutionContext context)
        {
            TestResult result = innerCommand.Execute(context);

            for (int attempt = 2; attempt <= tryCount && result.ResultState.Status == TestStatus.Failed; attempt++)
            {
                context.CurrentResult = context.CurrentTest.MakeTestResult();
                result = innerCommand.Execute(context);
            }

            context.CurrentResult = result;
            return result;
        }
    }
}
