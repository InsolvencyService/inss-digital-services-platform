using Microsoft.Extensions.Configuration;

namespace GovUk.Forms.Components.Extensions;

public static class ConfigurationManagerExtensions
{
    extension(ConfigurationManager configurationManager)
    {
        public TOptions Get<TOptions>(string key) where TOptions : new()
        {
            TOptions options = new();
            configurationManager.GetSection(key).Bind(options);
            return options;
        }
    }
}