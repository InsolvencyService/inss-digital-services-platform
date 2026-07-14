using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Domain;

// ReSharper disable UnusedParameter.Local

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class MockCaseReferenceClient : ICaseReferenceClient
{
    public Task<CaseDetailModel?> LookupCaseDetails(string caseReference)
    {
        const string unknownCaseReference = "CN12345678";
        return caseReference != unknownCaseReference
            ? Task.FromResult<CaseDetailModel?>(new CaseDetailModel
            {
                CaseReference = caseReference, CompanyName = "Springfield Nuclear"
            })
            : Task.FromResult<CaseDetailModel?>(null);
    }
}