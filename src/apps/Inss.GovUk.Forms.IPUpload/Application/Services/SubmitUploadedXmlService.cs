using System.Xml.Linq;
using GovUk.Forms.Domain;
using Inss.Common;
using Inss.Common.IPUpload;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Domain;

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
        XDocument document = FileHelper.GetXml(xml);
        bool isEmployeeUpload = FileHelper.IsEmployeeDocument(document);
        bool isApiSource = FileHelper.IsApiSource(document);
        
        SubmitIPUploadRequest request = new()
        {
            SessionId = sessionId, 
            Email = email, 
            FileContents = xml,
            IsEmployeeUpload = isEmployeeUpload, 
            IsApiSource = isApiSource
        };
        
        Result<SubmitIPUploadResponse> response = await _submitIPUploadSectionClient.SubmitAsync(request);

        return response.Match(success => success.Reference, error => throw new InvalidOperationException(error.Description));
    }
}