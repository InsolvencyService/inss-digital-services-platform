using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using GovUk.Forms.Application.Providers;

namespace GovUk.Forms.Infrastructure.Providers;

[ExcludeFromCodeCoverage]
public sealed class TestStaticContentProvider : IStaticContentProvider
{
    public Task<string> GetAsync(string key)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Resources.json");
        using StreamReader reader = new(path);
        
        JsonDocument jsonDocument = JsonDocument.Parse(reader.ReadToEnd());
        
        foreach (JsonElement element in jsonDocument.RootElement.EnumerateArray())
        {
            if (element.TryGetProperty("Key", out JsonElement keyElement))
            {
                if (keyElement.GetString()?.Equals(key, StringComparison.OrdinalIgnoreCase) == true &&
                    element.TryGetProperty("Value", out JsonElement valueElement))
                {
                    string filePath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "Resources", valueElement.GetString() ?? string.Empty);

                    if (File.Exists(filePath))
                    {
                        return Task.FromResult(File.ReadAllText(filePath));
                    }
                }
            }
        }
        
        throw new FileNotFoundException($"Unable to find the resource for {key}.");
    }
}