namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvPredProdConfig : IEnvironmentConfig
{
    public TestEnvironment EnvironmentType => TestEnvironment.PreProd;
    public string BaseUrl => "";
}
