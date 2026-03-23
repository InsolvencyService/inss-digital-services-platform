using GovUk.Forms.Application.Providers;

namespace GovUk.Forms.Infrastructure.Providers;

public sealed class TestStaticContentProvider : IStaticContentProvider
{
    public Task<string> GetAsync(string key)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", $"{key}.html");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Unable to find a HTML resource for {key}.");
        }
        
        using StreamReader reader = new(path);
        return Task.FromResult(reader.ReadToEnd());
    }
}