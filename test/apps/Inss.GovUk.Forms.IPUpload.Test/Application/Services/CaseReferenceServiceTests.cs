using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.Services;

public class CaseReferenceServiceTests
{
    private readonly CaseReferenceService _caseReferenceService;
    private readonly ICaseReferenceClient _caseReferenceClient;

    public CaseReferenceServiceTests()
    {
        _caseReferenceClient = Substitute.For<ICaseReferenceClient>();
        _caseReferenceService = new CaseReferenceService(_caseReferenceClient, Substitute.For<ILogger<CaseReferenceService>>());
    }

    [Fact]
    public async Task UnknownCaseReference_CheckExistsAsync_ReturnsFalse()
    {
        const string caseReference = "CN12345678";
        _caseReferenceClient.LookupCaseDetails(caseReference).Returns((CaseDetailModel?)null);

        CaseDetailModel? result = await _caseReferenceService.GetCaseDetailsAsync(caseReference);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task KnownCaseReference_CheckExistsAsync_ReturnsTrue()
    {
        const string caseReference = "CN12345678";
        _caseReferenceClient.LookupCaseDetails(caseReference).Returns(new CaseDetailModel());

        CaseDetailModel? result = await _caseReferenceService.GetCaseDetailsAsync(caseReference);

        Assert.NotNull(result);
    }
}