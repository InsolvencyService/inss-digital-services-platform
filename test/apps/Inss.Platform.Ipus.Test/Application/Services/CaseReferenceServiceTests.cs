using Inss.Platform.Ipus.Application.Clients;
using Inss.Platform.Ipus.Application.Services;
using Inss.Platform.Ipus.Application.Shared;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.Platform.Ipus.Test.Application.Services;

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
        CaseReferenceRequest request = new("CN12345678");
        _caseReferenceClient.LookupCaseDetails(request).Returns((CaseReferenceResponse?)null);

        CaseReferenceResponse? response = await _caseReferenceService.GetCaseDetailsAsync(request);

        Assert.Null(response);
    }
    
    [Fact]
    public async Task KnownCaseReference_CheckExistsAsync_ReturnsTrue()
    {
        CaseReferenceRequest request = new("CN12345678");
        _caseReferenceClient.LookupCaseDetails(request).Returns(new CaseReferenceResponse("CN12345678", "Springfield Nuclear"));

        CaseReferenceResponse? response = await _caseReferenceService.GetCaseDetailsAsync(request);
        Assert.NotNull(response);
        Assert.Equal("CN12345678", response.CaseReference);
        Assert.Equal("Springfield Nuclear", response.CompanyName);
    }
}