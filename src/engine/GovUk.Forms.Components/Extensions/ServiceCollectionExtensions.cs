using Azure.Identity;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Infrastructure.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Components.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
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