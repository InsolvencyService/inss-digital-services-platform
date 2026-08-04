using System.Xml.Linq;
using GovUk.Forms.Domain;
using Inss.Common;
using Inss.Common.IPUpload;
using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Application.Services;

public sealed class SubmitUploadedXmlService : ISubmitUploadedXmlService
{
    private readonly ISubmitIPUploadSectionClient _submitIPUploadSectionClient;
    private readonly IUploadContentBlobClient _uploadContentBlobClient;

    public SubmitUploadedXmlService(
        ISubmitIPUploadSectionClient submitIPUploadSectionClient, 
        IUploadContentBlobClient uploadContentBlobClient)
    {
        _submitIPUploadSectionClient = submitIPUploadSectionClient;
        _uploadContentBlobClient = uploadContentBlobClient;
    }

    public async Task<string> SubmitAsync(SectionModel section, string sessionId, string email)
    {
        string xml = await _uploadContentBlobClient.GetAsync(sessionId);
        XDocument document = XDocument.Parse(xml);
        bool isEmployeeUpload = FileHelper.IsEmployeeDocument(document);
        bool isApiSource = FileHelper.IsApiSource(document);
        
        SubmitIPUploadRequest request = new()
        {
            SessionId = sessionId, 
            Email = email, 
            IsEmployeeUpload = isEmployeeUpload, 
            IsApiSource = isApiSource
        };
        
        Result<SubmitIPUploadResponse> response = await _submitIPUploadSectionClient.SubmitAsync(request);

        return response.Match(success => success.Reference, error => throw new InvalidOperationException(error.Description));
    }
}