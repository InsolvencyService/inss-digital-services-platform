namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvPredProdConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.PreProd;
    public override string BaseUrl => "https://preprod.ipus.redundancy-payments.service.gov.uk/";
    public override string CosmosEndpoint => "https://cosmos-platform-uksouth-preprod.documents.azure.com:443/";
}
