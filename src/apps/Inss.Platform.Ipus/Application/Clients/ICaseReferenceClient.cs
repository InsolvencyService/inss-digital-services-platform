using Inss.Platform.Ipus.Application.Shared;

namespace Inss.Platform.Ipus.Application.Clients;

public interface ICaseReferenceClient
{
    Task<CaseReferenceResponse?> LookupCaseDetails(CaseReferenceRequest request);
}