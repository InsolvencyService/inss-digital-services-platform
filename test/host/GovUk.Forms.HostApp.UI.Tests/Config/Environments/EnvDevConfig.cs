namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public class EnvDevConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.Dev;
    public override string BaseUrl => "http://localhost:5056/ip-upload/redundancy-payment/declaration";
}
