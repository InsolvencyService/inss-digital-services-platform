namespace Inss.Platform.Submission.Service.IPUpload.Clients;

public sealed class MockUploadContentBlobClient : IUploadContentBlobClient
{
    public Task<string> GetAsync(string sessionId)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ipus", $"{sessionId}.xml");
        return Task.FromResult(File.ReadAllText(path));
    }
}