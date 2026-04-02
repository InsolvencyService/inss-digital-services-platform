# Hosting App

In the _host_ folder there is a hosting project called _GovUk.Forms.HostApp_. This is the single app that runs the forms.

It utilises configuration to decide what apps, under the _apps_ folder should be loaded.

Each app project is referenced by the host but that does not mean it will be used. Think of it as registration.

The actual decision to load apps is based upon the _DOTNET_HOSTINGSTARTUPASSEMBLIES_ environment variable.

An example is defined in the host project within the _launchsettings.json_ file:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "",
      "applicationUrl": "http://localhost:5056",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey=your-key-here",
        "DOTNET_HOSTINGSTARTUPASSEMBLIES": "GovUk.Forms.Components;Demo.GovUk.Forms.AboutYou"
        // AVAILABLE ONES ... "DOTNET_HOSTINGSTARTUPASSEMBLIES": "GovUk.Forms.Components;Demo.GovUk.Forms.AboutYou;Demo.GovUk.Forms.Bankruptcy;Demo.GovUk.Forms.Business;Inss.GovUk.Forms.IPUpload"
      }
    }
  }
}
```

**You must** define the _GovUk.Forms.Components_ assembly first! This bootstraps all the shared components _before_ the 
app is registered.

You then add your assembly app - with semi-colon to the list.

In each app, you must implement the _IHostingStartup_ interface. This is the standard ASP.NET extension that allows libraries
to register configuration during the startup process.

An example is:

```c#
using GovUk.Forms.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Demo.GovUk.Forms.Bankruptcy.StartupConfiguration))]

namespace Demo.GovUk.Forms.Bankruptcy;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            
            YourBankruptcyFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}
```

**Note that** the assembly attribute must be defined. This is required to allow the ASP.NET startup process to instanciate 
and invoke the implementation of the _IHostingStartup_ interface. **You will have** one per app.

You will also need to implement the _IWebRoot_ interface. This is used to define you root path and allow the components 
project to create all the routes for the form(s). An example is:

```c#
public sealed class WebRoot : IWebRoot
{
    public ContentPath Root => "/bankruptcy";
}
```

Using the above pattern allows us to have a single host but create multiple instances of it in Azure within one or more 
app service plans. Then it is a simple case of deciding - using the environment variable above, what apps to host. **This**
can then be controlled as part of a deployment process.