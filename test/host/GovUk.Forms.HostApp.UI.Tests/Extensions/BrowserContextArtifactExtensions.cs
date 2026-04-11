using GovUk.Forms.HostApp.UI.Tests.Helpers;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class BrowserContextArtifactExtensions
{
    extension(IBrowserContext browserContext)
    {
        public async Task StopTracingToArtifactsAsync(
            TestArtifacts artifacts,
            IReqnrollOutputHelper outputHelper)
        {
            ArgumentNullException.ThrowIfNull(browserContext);
            ArgumentNullException.ThrowIfNull(artifacts);
            ArgumentNullException.ThrowIfNull(outputHelper);

            try
            {
                await browserContext.Tracing.StopAsync(new TracingStopOptions
                {
                    Path = artifacts.TracePath
                });

                if (File.Exists(artifacts.TracePath))
                {
                    outputHelper.AddAttachment(artifacts.TracePath);
                }
            }
            catch (Exception ex)
            {
                outputHelper.WriteLine($"[Tracing Error] {ex.Message}");
            }
        }
    }
}
