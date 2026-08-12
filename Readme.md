# Platform Apps

## Overview

This project provides a component library with to build forms-based platform apps that are UK Government styled.

This project levearages the NuGet package maintained by https://github.com/x-govuk/govuk-frontend-aspnetcore and builds a 
layer on top to provide a form-based framework.

It uses clean code architecture to separate concerns and define domain, application and infrastucture functions to the components library.

## Clean Code Architecture

Below is a basic diagram of the components that form the architecture of the project:

![image](doc/Clean%20Code%20Architecture.png)

The following sections discuss each in more detail.

### Domain

The _domain_ layer is where we define our _models_ and supporting rules and helpers. This has no dependencies on other projects
but will be consumed by the application layer and, indirectly the components (presentation layer).

This project also has some specific functionality to handle serialization of the domain, enums that are used and _primitives_ to
ensure domain _identifiers_ are not mixed up.

### Application

This layer provides the core functionality for the application. This is where the flowchart behaviour is defined, the extension points
that allow the components or web app to extend such as the form factory for creating instances of the form or the processing or decision
making of the flowchart.

### Infrastructure

This layer provides out external implementation such as where to store and retrieve the form for a user, the providing of user identity
and, as required, external calls to databases or 3rd party services such as bank validation or other internal APIs.

**Its is important** to note that the presentation layer _only_ bootstraps this layer via the provided service extension. All other operartions
are via indirect calls through the _application_ layer via interfaces and abstractions.

### Component

This layer is part of the presentation layer and is what allows other projects, such as the demo web app to consume the components and build
working forms. Itself provides web app build and web app extensions and will resolve the form via thwe factory, upon startup and configure the
routing for the whole form, validate the form and also the flowchart for each section.

### Host App

This is a host project which _hosts_ an app and is a thin layer. The principal is based upon a plug-in architecture where
the host app is configured to run a specific app and bootstrap it through dependency injection, of known interfaces and abstractions.

### App Settings & User Secrets

Each app has its own _appsettings.app.json_ file. This uses a convention-based approach where if your app is _Inss.GovUk.Forms.Fip_ 
then the json file will be _appsettings.fip.json_.

There is a line included in the _StartupConfiguration_ for each app:

```c#
builder.AddDeveloperConfig<StartupConfiguration>();
```

which will bootstrap the config for the app within the host app and also include the project user secrets.

This means that the host app _appsettings.json_ is largely empty and there are no _user secrets_ either in the host app.

This allows each app to define its configuration, isolated from other apps.

The use of _user secrets_ is to avoid accidental check in of sensitive keys, secrets and IDs. GitHub will do a good job of
rejecting pushes with sensitive data but, as a principal, we avoid defining values in the _appsettings.json_ in ther first place.

### Mocks

There is a covention-based approach to switching on _mocks_ of services for development only. Any app settins section that 
may have a mock, will allow you to add _UseMock_ in the app settings/user secrets and will look for it when bootstrapping 
the app.

For example, in IPUS, we need to store the uploaded XML to blob but locally that may not be possible due to firewall restrictions.

A mock exists to support this which uses a local folder. You can configure the _UploadBlob_ section in your user secrets like:

```json
"UploadBlob": {
    "ConnectionString": "c:/dev/ipus-blobs",
    "UseMock": true
  },
```

**The UseMock** is an extra property that does not belong to the options but will be checked for such that the start-up code:

```c#
private static void RegisterUploadContentBlobClient(WebHostBuilderContext context, IServiceCollection services)
{
    UploadBlobOptions uploadBlobOptions = context.Configuration.GetSection("UploadBlob").Get<UploadBlobOptions>()!;
    
    if (context.UseMock("UploadBlob"))
    {
        services.AddSingleton<IUploadContentBlobClient>(_ => new MockUploadContentBlobClient(uploadBlobOptions.ConnectionString));
    }
    else
    {
        services.AddTransient<IUploadContentBlobClient>(
            _ => new UploadContentBlobClient(new BlobServiceClient(uploadBlobOptions.ConnectionString)));
    }
}
```

will use a helper extension called _UseMock_ with the section, to determine whether to use a mock implementation of the interface.

**Note that** this **only** applies to development as the the following logic is enforced:

```c#
public bool UseMock(string key)
{
    return context.HostingEnvironment.IsDevelopment() &&
           bool.TryParse(context.Configuration[$"{key}:UseMock"], out bool useMock) && useMock;
}
```

## Links

[GovUK ASP.NET](https://github.com/x-govuk/govuk-frontend-aspnetcore)

[Insolvency Service Prototypes](https://digital-services-prototypes.azurewebsites.net)

[Clean Code Architecture Diagram](doc/Clean%20Code%20Architecture.drawio)