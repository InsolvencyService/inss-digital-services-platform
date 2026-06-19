namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvSitConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.SIT;
    public override string BaseUrl => "https://sit.ipus.redundancy-payments.service.gov.uk";
}
