namespace Inss.Platform.Submission.Service.IPUpload.Clients;

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
}