using Inss.Platform.Ipus.Application.Clients;
using Inss.Platform.Ipus.Application.Shared;
using Inss.Platform.Ipus.Extensions;

namespace Inss.Platform.Ipus.Application.Services;

public sealed class CaseReferenceService : ICaseReferenceService
{
    private readonly ICaseReferenceClient _caseReferenceClient;
    private readonly ILogger<CaseReferenceService> _logger;

    public CaseReferenceService(ICaseReferenceClient caseReferenceClient, ILogger<CaseReferenceService> logger)
    {
        _caseReferenceClient = caseReferenceClient;
        _logger = logger;
    }

    public async Task<CaseReferenceResponse?> GetCaseDetailsAsync(CaseReferenceRequest request)
    {
        _logger.LookupCaseDetailsExists(request.CaseReference);
         
        return await _caseReferenceClient.LookupCaseDetails(request);
    }
}