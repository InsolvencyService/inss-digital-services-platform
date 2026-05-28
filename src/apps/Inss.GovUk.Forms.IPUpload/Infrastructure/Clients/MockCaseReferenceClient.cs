using Inss.GovUk.Forms.IPUpload.Application.Clients;
// ReSharper disable UnusedParameter.Local

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class MockCaseReferenceClient : ICaseReferenceClient
{
    public MockCaseReferenceClient(HttpClient client)
    {
    }
    
    public Task<bool> CheckExistsAsync(string caseReference)
    {
        Console.WriteLine("Calling RPS...");
        const string unknownCaseReference = "CN12345678";
        return Task.FromResult(caseReference != unknownCaseReference);
    }
}