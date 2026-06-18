using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.Services;

public interface ICaseReferenceService
{
    Task<CaseDetailModel?> GetEmployerDetailsAsync(string caseReference);
}