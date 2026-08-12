using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class MockUploadContentBlobClient : IUploadContentBlobClient
{
    private readonly string _rootPath;

    public MockUploadContentBlobClient(string rootPath)
    {
        _rootPath = rootPath;
    }
    
    public Task<string> GetAsync(string sessionId)
    {
        string path = Path.Combine(_rootPath, $"{sessionId}.xml");
        return Task.FromResult(File.ReadAllText(path));
    }
    
    public Task SaveAsync(string xml, string sessionId)
    {
        string path = Path.Combine(_rootPath, $"{sessionId}.xml");
        File.WriteAllText(path, xml);
        return Task.CompletedTask;
    }
    
    public Task RemoveAsync(string sessionId)
    {
        string path = Path.Combine(_rootPath, $"{sessionId}.xml");

        if (File.Exists(path))
        {
            File.Delete(path);
        }
        
        return Task.CompletedTask;
    }
}