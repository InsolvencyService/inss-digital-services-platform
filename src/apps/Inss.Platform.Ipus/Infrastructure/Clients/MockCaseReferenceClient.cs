// ReSharper disable UnusedParameter.Local
using Inss.Platform.Ipus.Application.Clients;
using Inss.Platform.Ipus.Application.Shared;

namespace Inss.Platform.Ipus.Infrastructure.Clients;

public sealed class MockCaseReferenceClient : ICaseReferenceClient
{
    public Task<CaseReferenceResponse?> LookupCaseDetails(CaseReferenceRequest request)
    {
        const string unknownCaseReference = "CN12345678";
        return request.CaseReference != unknownCaseReference
            ? Task.FromResult<CaseReferenceResponse?>(new CaseReferenceResponse(request.CaseReference, "Springfield Nuclear"))
            : Task.FromResult<CaseReferenceResponse?>(null);
    }
}