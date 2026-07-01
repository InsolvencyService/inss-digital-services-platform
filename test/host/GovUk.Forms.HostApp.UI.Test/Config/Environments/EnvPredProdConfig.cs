namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvPredProdConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.PreProd;
    public override string BaseUrl => "";
    public override string CosmosEndpoint => "";
}
