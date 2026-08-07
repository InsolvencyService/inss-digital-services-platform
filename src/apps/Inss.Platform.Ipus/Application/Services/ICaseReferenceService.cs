using Inss.Platform.Ipus.Application.Shared;

namespace Inss.Platform.Ipus.Application.Services;

public interface ICaseReferenceService
{
    Task<CaseReferenceResponse?> GetCaseDetailsAsync(CaseReferenceRequest request);
}