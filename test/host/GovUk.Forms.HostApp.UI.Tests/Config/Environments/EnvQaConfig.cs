namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public class EnvQaConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.QA;
    public override string BaseUrl => "https://digital-services-prototypes.azurewebsites.net/ipservice/ip-upload/index";
}
