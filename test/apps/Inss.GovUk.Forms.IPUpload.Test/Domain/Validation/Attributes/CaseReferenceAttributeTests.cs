using System.ComponentModel.DataAnnotations;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Attributes;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Attributes;

public class CaseReferenceAttributeTests
{
    private readonly CaseReferenceAttribute _caseReferenceAttribute = new(CaseRefErrorKey);
    private readonly ICaseReferenceService _caseReferenceService = Substitute.For<ICaseReferenceService>();
    private readonly ValidationContext _validationContext = new(CaseReference); 
    private const string CaseRefErrorKey = "CaseRefErrorKey";
    private const string CaseReference = "CN12345678";

    public CaseReferenceAttributeTests()
    {
        ServiceCollection services = [];
        services.AddSingleton(_caseReferenceService);
        _validationContext.InitializeServiceProvider(services.BuildServiceProvider().GetService);
    }
    
    [Fact]
    public void UnknownCaseReference_Validate_AssignsErrorKeyToErrorMessage()
    {
        _caseReferenceService.CheckExistsAsync(CaseReference).Returns(false);

        Assert.Throws<ValidationException>(() => _caseReferenceAttribute.Validate(CaseReference, _validationContext));
        
        Assert.Equal(CaseRefErrorKey, _caseReferenceAttribute.ErrorMessage);
    }
    
    [Fact]
    public void KnownCaseReference_Validate_DoesNotAssignErrorKeyToErrorMessage()
    {
        _caseReferenceService.CheckExistsAsync(CaseReference).Returns(true);

        _caseReferenceAttribute.Validate(CaseReference, _validationContext);
        
        Assert.Null(_caseReferenceAttribute.ErrorMessage);
    }
}