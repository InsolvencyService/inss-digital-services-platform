namespace GovUk.Forms.HostApp.UI.Test.Factories;

public static class Rp14aFileFactory
{
    public static async Task<string> CreateAsync(string xml, string fileName = "rp14a.xml")
    {
        string folder = Path.Combine(Path.GetTempPath(), "rp14a-test-files");
        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, $"{Guid.NewGuid()}_{fileName}");

        await File.WriteAllTextAsync(path, xml);

        return path;
    }
}
