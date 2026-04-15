namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public static class EnvironmentConfigFactory
{
    private static readonly Lazy<IEnvironmentConfig> _environmentConfig =
        new(BuildEnvironmentConfig, LazyThreadSafetyMode.ExecutionAndPublication);

    public static IEnvironmentConfig EnvironmentConfig => _environmentConfig.Value;

    public static TestEnvironment CurrentEnvironment => EnvironmentConfig.EnvironmentType;

    private static IEnvironmentConfig BuildEnvironmentConfig()
    {
        TestEnvironment environment = GetEnvironment();
        return Create(environment);
    }

    private static TestEnvironment GetEnvironment()
    {
        string? environment =
            Environment.GetEnvironmentVariable("TEST_ENVIRONMENT")
            ?? TestConfigReader.Settings.TestEnvironment;

        if (string.IsNullOrWhiteSpace(environment))
        {
            throw new InvalidOperationException(
                "Test environment not specified. Set 'TestSettings:TestEnvironment' in appsettings.json " +
                "or the 'TEST_ENVIRONMENT' environment variable.");
        }

        if (!Enum.TryParse(environment, ignoreCase: true, out TestEnvironment result))
        {
            throw new InvalidOperationException(
                $"Invalid test environment '{environment}'. Valid values are: " +
                string.Join(", ", Enum.GetNames<TestEnvironment>()));
        }

        return result;
    }

    public static IEnvironmentConfig Create(TestEnvironment environmentType)
    {
        return environmentType switch
        {
            TestEnvironment.QA => new EnvQaConfig(),
            TestEnvironment.Dev => new EnvDevConfig(),
            TestEnvironment.ST => new EnvStConfig(),
            TestEnvironment.Prod => new EnvProdConfig(),
            _ => throw new ArgumentOutOfRangeException(nameof(environmentType),
                $"Unsupported environment type: {environmentType}")
        };
    }
}
