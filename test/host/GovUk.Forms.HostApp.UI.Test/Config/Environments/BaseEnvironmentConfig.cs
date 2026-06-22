namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public enum TestEnvironment
{
    QA,
    Dev,
    SIT,
    PreProd
}
public abstract class BaseEnvironmentConfig : IEnvironmentConfig
{
    public abstract TestEnvironment EnvironmentType { get; }
    public abstract string BaseUrl { get; }
}
