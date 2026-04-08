namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;



public enum TestEnvironment
{
    QA,
    Dev,
    ST,
    Prod
}
public abstract class BaseEnvironmentConfig : IEnvironmentConfig
{
    public abstract TestEnvironment EnvironmentType { get; }
    public abstract string BaseUrl { get; }
}
