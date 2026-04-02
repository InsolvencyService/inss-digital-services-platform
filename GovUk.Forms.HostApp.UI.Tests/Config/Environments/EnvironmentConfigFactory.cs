namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public static class EnvironmentConfigFactory
{

    public static IEnvironmentConfig GetEnvironmentConfig()
    {
        TestEnvironment environment = GetEnvironmente();

        return Create(environment);
    }



    private static TestEnvironment GetEnvironmente()
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
            TestEnvironment.Local => new EnvLocalConfig(),
            TestEnvironment.Dev => new EnvDevConfig(),
            TestEnvironment.Test => new EnvTestConfig(),
            TestEnvironment.Prod => new EnvProdConfig(),
            _ => throw new ArgumentException($"Unsupported environment type: {environmentType}")
        };
    }
}
