using System.ComponentModel.DataAnnotations;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Xunit;
// ReSharper disable RedundantExplicitParamsArrayCreation

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation;

public class ValidationContextExtensionTests
{
    private readonly ValidationContext _validationContext = new(CaseReference); 
    private const string CaseReference = "CN12345678";
    
    [Fact]
    public void NotVisited_CreateValidationResult_ReturnsResult()
    {
        ValidationResult? result = _validationContext.CreateValidationResult("UnknownCaseReferenceKey", "CaseReference", ["CaseReference"]);

        Assert.NotNull(result);
    }
    
    [Fact]
    public void Visited_CreateValidationResult_ReturnsNull()
    {
        _validationContext.CreateValidationResult("UnknownCaseReferenceKey", "CaseReference", ["CaseReference"]);
        
        ValidationResult? result = _validationContext.CreateValidationResult("UnknownCaseReferenceKey", "CaseReference", ["CaseReference"]);

        Assert.Null(result);
    }
}