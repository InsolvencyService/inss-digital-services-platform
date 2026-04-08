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
    
    public string Name => "Bankruptcy";
}
```

Using the above pattern allows us to have a single host but create multiple instances of it in Azure within one or more 
app service plans. Then it is a simple case of deciding - using the environment variable above, what apps to host. **This**
can then be controlled as part of a deployment process.

## Start Page

The main host app looks for a start partial view so each app will need to create one with the name _Start.cshtml.

## Authentication

By default, all the apps will run anonymous. To override this, you can configure the _BrokerOptions_ property _IdentityProvider_ 
in the user secrets for the host app:

```json
{
  "Broker": {
    "ClientId": "123",
    "IdentityProvider": "Rps",
    "JwtPublicKey": "-----BEGIN PUBLIC KEY----- MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtkw6a1gJg95FMaDBbET8 Yfdk9P1Pl7DMKok+868r7g2V9BuwbwoSjIBGXAqJuWoYX6NChkjHvO9dKTWHTqpg C0wDwNFdp6QGEZpqPj2n6PTGTyUpTJttf4AHr9sVDIhCe5aEipBVcbrApIsShSy7 /US763a7T/BLpyhZVaysgnUE19P7zM91Ehqf0vPlT0QjHQsHw3E2x7vr4J9NZjIM OoBlwL910KZra6g7Bwh1jGY9GDCiON0DnZkjp5BzxMsBHrI3lFBZHXNKbpfRZrWY RuaUaxHOSt2OI0nrd2i9ZCARXPmyHOfkskZT5GUy8wiGjA7mtF8pOIsHgKMDrjyW pwIDAQAB -----END PUBLIC KEY-----"
  }
}
```

**All your** sensitve broker configuration should reside here.

**Note that** the _IdentityProvider_ property is optional so you only need to configure this, if you need to test authentication.

If you want to run authentication, you will need to run the broker at the very least, and if you are authenticaing with RPS then
the RPS identity provider will also need to be running.