namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvQaConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.QA;
    public override string BaseUrl => "";
}
