using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
// ReSharper disable UnusedParameter.Local

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class MockSubmitIPUploadSectionClient : ISubmitIPUploadSectionClient
{
    public MockSubmitIPUploadSectionClient(HttpClient client)
    {
    }
    
    public Task SubmitAsync(SectionModel section, string userSessionId)
    {
        Console.WriteLine("Calling submission service...");
        return Task.CompletedTask;
    }
}