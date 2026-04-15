using GovUk.Forms.HostApp.UI.Test.Models.Settings;
using Microsoft.Extensions.Configuration;

namespace GovUk.Forms.HostApp.UI.Test.Config;

public static class TestConfigReader
{
    public static TestSettings Settings { get; }
    private static readonly IConfigurationRoot _config;

    static TestConfigReader()
    {

        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();


        Settings = _config
                    .GetSection("TestSettings")
                    .Get<TestSettings>()
                    ?? throw new InvalidOperationException(
                        "TestSettings configuration section is missing or invalid.");



        if (string.IsNullOrWhiteSpace(Settings.TestEnvironment))
        {
            throw new InvalidOperationException("TestEnvironment is not configured.");
        }
    }
}
