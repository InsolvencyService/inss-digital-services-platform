namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvProdConfig : IEnvironmentConfig
{
    public TestEnvironment EnvironmentType => TestEnvironment.Prod;
    public string BaseUrl => "";
}
