using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class CaseReferenceClient : ICaseReferenceClient
{
    private readonly HttpClient _client;

    public CaseReferenceClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<bool> CheckExistsAsync(string caseReference)
    {
        // TODO: Integrate with the relay (sender)
        
        Console.WriteLine("Calling RPS...");
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        
        const string unknownCaseReference = "CN12345678";
        return caseReference != unknownCaseReference;
    }
}