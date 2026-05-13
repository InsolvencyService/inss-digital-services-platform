using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiAddressValidatorTests
{
    private readonly RP14ApiAddressValidator _validator = new("Any");
    
    [Fact]
    public void InvalidAddressLines_TestValidate_ReturnsInvalidResult()
    {
        AddressType model = new()
        {
            Line = ["Line1", "Line2", "Line3", "Line4", "Line5"],
            Town = "Springfield",
            County = "Nevada",
            Postcode = "TN33 0DN",
            Country = "USA"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidAddressLineLength_TestValidate_ReturnsInvalidResult()
    {
        AddressType model = new()
        {
            Line = [new string('X', 36)],
            Town = "Springfield",
            County = "Nevada",
            Postcode = "TN33 0DN",
            Country = "USA"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidTownLength_TestValidate_ReturnsInvalidResult()
    {
        AddressType model = new()
        {
            Line = ["123 Evergreen Terrace"],
            Town = new string('X', 36),
            County = "Nevada",
            Postcode = "TN33 0DN",
            Country = "USA"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidCountyLength_TestValidate_ReturnsInvalidResult()
    {
        AddressType model = new()
        {
            Line = ["123 Evergreen Terrace"],
            Town = "Springfield",
            County = new string('X', 36),
            Postcode = "TN33 0DN",
            Country = "USA"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidPostcodeLength_TestValidate_ReturnsInvalidResult()
    {
        AddressType model = new()
        {
            Line = ["123 Evergreen Terrace"],
            Town = "Springfield",
            County = "Nevada",
            Postcode = new string('X', 11),
            Country = "USA"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidCountryLength_TestValidate_ReturnsInvalidResult()
    {
        AddressType model = new()
        {
            Line = ["123 Evergreen Terrace"],
            Town = "Springfield",
            County = "Nevada",
            Postcode = "TN33 0DN",
            Country = new string('X', 11)
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidAddress_TestValidate_ReturnsValidResult()
    {
        AddressType model = new()
        {
            Line = ["123 Evergreen Terrace"],
            Town = "Springfield",
            County = "Nevada",
            Postcode = "TN33 0DN",
            Country = "USA"
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}