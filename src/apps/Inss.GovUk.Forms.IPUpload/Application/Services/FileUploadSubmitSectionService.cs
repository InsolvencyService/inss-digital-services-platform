using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Application.Services;

public sealed class FileUploadSubmitSectionService : ISubmitSectionService
{
    private readonly ISubmitIPUploadSectionClient _submitSectionClient;

    public FileUploadSubmitSectionService(ISubmitIPUploadSectionClient submitSectionClient)
    {
        _submitSectionClient = submitSectionClient;
    }
    
    public async Task SubmitAsync(SectionModel section, string userSessionId)
    {
        await _submitSectionClient.SubmitAsync(section, userSessionId);
    }
}