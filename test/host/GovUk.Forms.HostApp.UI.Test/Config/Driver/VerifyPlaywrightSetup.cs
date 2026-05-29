using System.Runtime.CompilerServices;

namespace GovUk.Forms.HostApp.UI.Test.Config.Driver;

public static class VerifyPlaywrightSetup
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyPlaywright.Initialize();
    }
}
