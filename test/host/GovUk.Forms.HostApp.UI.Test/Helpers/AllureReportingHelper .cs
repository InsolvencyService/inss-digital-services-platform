using Allure.Net.Commons;
using System.Runtime.CompilerServices;
using System.Text;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public sealed class AllureReportingHelper : IAllureReportingHelper
{
    public async Task StepAsync(
        string stepName,
        Func<Task> action,
        [CallerMemberName] string caller = "")
    {
        await AllureApi.Step(stepName, async () =>
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                AttachText(
                    "Exception Details",
                    $"""
                    Step: {stepName}
                    Caller: {caller}
                    Error: {ex}
                    """);

                throw;
            }
        });
    }

    public async Task<T> StepAsync<T>(
        string stepName,
        Func<Task<T>> action)
    {
        return await AllureApi.Step(stepName, action);
    }

    public async Task AttachScreenshotAsync(
        IPage page,
        string name = "Screenshot",
        bool fullPage = true)
    {
        if (page.IsClosed)
        {
            return;
        }

        try
        {
            byte[] screenshot = await page.ScreenshotAsync(new PageScreenshotOptions
            {
                FullPage = fullPage,
                Type = ScreenshotType.Png
            });

            AllureApi.AddAttachment(
                Sanitize(name),
                "image/png",
                screenshot);
        }
        catch (Exception ex)
        {
            AttachText("Screenshot Failed", ex.ToString());
        }
    }

    public void AttachText(string name, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        AllureApi.AddAttachment(
            Sanitize(name),
            "text/plain",
            Encoding.UTF8.GetBytes(content));
    }

    public void AttachFile(
        string filePath,
        string? name = null,
        string mimeType = "text/plain")
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        AllureApi.AddAttachment(
            Sanitize(name ?? Path.GetFileName(filePath)),
            mimeType,
            File.ReadAllBytes(filePath));
    }

    public void AddParameter(string name, object? value)
    {
        AllureApi.AddTestParameter(name, value?.ToString() ?? "null");
    }

    private static string Sanitize(string name)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return name;
    }

    public static void WriteEnvironmentProperties(
              Dictionary<string, string> properties)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        string allureDir = Path.Combine(baseDir, "allure-results");

        Directory.CreateDirectory(allureDir);

        string path = Path.Combine(allureDir, "environment.properties");

        IEnumerable<string> lines = properties.Select(kv => $"{kv.Key}={kv.Value}");

        File.WriteAllLines(path, lines, Encoding.UTF8);
    }
}
