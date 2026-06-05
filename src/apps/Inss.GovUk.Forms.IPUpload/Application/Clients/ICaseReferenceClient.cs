using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.Clients;

public interface ICaseReferenceClient
{
    Task<CaseDetailModel?> LookupCaseDetails(string caseReference);
}