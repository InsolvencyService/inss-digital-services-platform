namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public static class EnvironmentConfigFactory
{
    public static IEnvironmentConfig EnvironmentConfig => GetEnvironmentConfig();
    public static TestEnvironment CurrentEnvironment => EnvironmentConfig.EnvironmentType;
    public static IEnvironmentConfig GetEnvironmentConfig()
    {
        TestEnvironment environment = GetEnvironment();

        return Create(environment);
    }



    private static TestEnvironment GetEnvironment()
    {
        string environment = TestContext.Parameters.Get("TestEnvironment") ??
            Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ??
            throw new InvalidOperationException("Test environment not specified. Please set the 'TestEnvironment' parameter or the 'TEST_ENVIRONMENT' environment variable.");

        return Enum.Parse<TestEnvironment>(environment, ignoreCase: true);
    }

    public static IEnvironmentConfig Create(TestEnvironment environmentType)
    {
        return environmentType switch
        {
            TestEnvironment.QA => new EnvQaConfig(),
            TestEnvironment.Dev => new EnvDevConfig(),
            TestEnvironment.ST => new EnvStConfig(),
            TestEnvironment.Prod => new EnvProdConfig(),
            _ => throw new ArgumentException($"Unsupported environment type: {environmentType}")
        };
    }
}
