using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class SubmitIPUploadSectionClient : ISubmitIPUploadSectionClient
{
    private readonly HttpClient _client;

    public SubmitIPUploadSectionClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task SubmitAsync(SectionModel section, string userSessionId)
    {
        Console.WriteLine("Calling dynamics...");
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
    }
}