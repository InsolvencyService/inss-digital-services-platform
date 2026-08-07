using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Inss.Platform.Component.Binding;
using Inss.Platform.Component.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Extensions;

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

            services.AddControllersWithViews(o => o.ModelBinderProviders.Insert(0, new PageComponentBinderProvider()));
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationFormats.Add("/Views/Components/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/Components/Parts/{0}.cshtml");
            });
            services.AddHttpClient();
            services.AddGovUkFrontend();
            services.AddHealthChecks();
            services.AddOpenTelemetry().UseAzureMonitor();
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
}