using Azure.Identity;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Components.Binding;
using GovUk.Forms.Components.Controllers;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using GovUk.Forms.Infrastructure.Extensions;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Components.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddComponents(IConfiguration configuration)
        {
            services.AddOptions<HeaderOptions>()
                .Bind(configuration.GetSection("Header"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<FooterOptions>()
                .Bind(configuration.GetSection("Footer"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<AnalyticsOptions>()
                .Bind(configuration.GetSection("Analytics"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            IMvcBuilder mvcBuilder = services
                .AddControllersWithViews(o => o.ModelBinderProviders.Insert(0, new ContentModelBinderProvider()))
                .AddApplicationPart(typeof(FormController).Assembly);
            RemoveNonHostedDiscoveredParts(mvcBuilder);

            services.AddSingleton<IContentBinderFactory, ContentBinderFactory>();
            services.AddSingleton<IContentBinder, DefaultContentBinder>();
            services.AddKeyedSingleton<IContentBinder, FileContentBinder>(typeof(FileUploadModel).FullName);
            services.AddSingleton<ITypeNameResolver, TypeNameResolver>();
            services.AddHttpClient();
            services.AddGovUkFrontend();
            services.AddHealthChecks();
            return services;
        }
        
        public IServiceCollection AddFormEngine(IConfiguration configuration)
        {
            services.AddApplication();
            services.AddInfrastructure(configuration);
            return services;
        }
        
        public IServiceCollection AddAppDataProtection(IConfiguration configuration)
        {
            // Below manages the shared data protection mechanism which is used by auth correlation cookies among other functions.
            // The important part is the app name (SetApplicationName) as this ensures that the cross app cookies can be decrypted.
            // It uses managed identities which means the app must have the appropriate perms on blob (data storage contributor) and
            // encrypt/unwrap etc perms on key vault. These perms are granted and managed in the IaC.
            
            AppDataProtectionOptions options = configuration.BindAndValidate<AppDataProtectionOptions>("DataProtection");
            Uri blobKeyPath = new($"https://{options.StorageAccountName}.blob.core.windows.net/{options.StorageAccountBlobName}/keys.xml");
            Uri keyVaultKeyPath = new($"https://{options.KeyVaultName}.vault.azure.net/keys/{options.KeyVaultKeyName}");
            DefaultAzureCredential credential = new();
            services
                .AddDataProtection()
                .SetApplicationName(options.AppName)
                .PersistKeysToAzureBlobStorage(blobKeyPath, credential)
                .ProtectKeysWithAzureKeyVault(keyVaultKeyPath, credential);

            return services;
        }
    }
    
    private static string[] GetHostedAssemblyNames()
    {
        string environmentName = Environment.GetEnvironmentVariable("DOTNET_HOSTINGSTARTUPASSEMBLIES")!;
        var hostedAssemblies = environmentName
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        hostedAssemblies.Add("GovUk.Forms.HostApp");
        hostedAssemblies.Add("GovUk.Frontend.AspNetCore");
        return hostedAssemblies.ToArray();
    }

    private static void RemoveNonHostedDiscoveredParts(IMvcBuilder mvcBuilder)
    {
        string[] hostedAssemblyNames = GetHostedAssemblyNames();
        ApplicationPartManager partManager = mvcBuilder.PartManager;
        ApplicationPart[] applicationPartsToRemove = partManager.ApplicationParts.Where(
            part => !hostedAssemblyNames.Contains(part.Name)).ToArray();
            
        foreach (var part in applicationPartsToRemove)
        {
            partManager.ApplicationParts.Remove(part);
        }
    }
}