using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Application.Services;
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WithCaseReference_CheckExistsAsync_ReturnsClientResponse(bool exists)
    {
        const string caseReference = "CN12345678";
        _caseReferenceClient.CheckExistsAsync(caseReference).Returns(exists);
        
        bool result = await _caseReferenceService.CheckExistsAsync(caseReference);
        
        Assert.Equal(exists, result);
    }
}