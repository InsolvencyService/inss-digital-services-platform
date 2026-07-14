using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Extensions;
using Microsoft.Extensions.Logging;

namespace Inss.GovUk.Forms.IPUpload.Application.Services;

public sealed class CaseReferenceService : ICaseReferenceService
{
    private readonly ICaseReferenceClient _caseReferenceClient;
    private readonly ILogger<CaseReferenceService> _logger;

    public CaseReferenceService(ICaseReferenceClient caseReferenceClient, ILogger<CaseReferenceService> logger)
    {
        _caseReferenceClient = caseReferenceClient;
        _logger = logger;
    }

    public async Task<CaseDetailModel?> GetCaseDetailsAsync(string caseReference)
    {
        _logger.LookupCaseDetailsExists(caseReference);
         
        return await _caseReferenceClient.LookupCaseDetails(caseReference);
    }
}