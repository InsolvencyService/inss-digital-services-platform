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

    public SubmitUploadedXmlService(ISubmitIPUploadSectionClient submitIPUploadSectionClient)
    {
        _submitIPUploadSectionClient = submitIPUploadSectionClient;
    }

    public async Task<string> SubmitAsync(SectionModel section, string userId)
    {
        XmlFileUploadModel fileUpload = section.Pages.GetFirstOf<XmlFileUploadModel>();
        XDocument document = FileHelper.GetXml(fileUpload.Contents);
        bool isEmployeeUpload = FileHelper.IsEmployeeDocument(document);
        
        SubmitIPUploadRequest request = new() { UserId = userId, FileContents = fileUpload.Contents, IsEmployeeUpload = isEmployeeUpload };
        
        Result<SubmitIPUploadResponse> response = await _submitIPUploadSectionClient.SubmitAsync(request);

        return response.Match(success => success.Reference, error => throw new InvalidOperationException(error.Description));
    }
}