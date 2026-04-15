namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public interface IEnvironmentConfig
{
    TestEnvironment EnvironmentType { get; }
    string BaseUrl { get; }
}
