using System.Globalization;
using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class RP14AApiArrearsOfPayValidatorTests
{
    private readonly RP14AApiArrearsOfPayValidator _validator = new();
    
    [Fact]
    public void InvalidArrearsOfPayedOwed_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployeePayDetailsArrearsOfPayPeriod model = new()
        {
            AOPOwed = 100.234M,
            Period = new PeriodType
            {
                StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
                EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            }
        };
        
        TestValidationResult<RP14AEmployeePayDetailsArrearsOfPayPeriod>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidPeriodDates_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployeePayDetailsArrearsOfPayPeriod model = new()
        {
            AOPOwed = 100.23M,
            Period = new PeriodType
            {
                StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
                EndDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture)
            }
        };
        
        TestValidationResult<RP14AEmployeePayDetailsArrearsOfPayPeriod>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidPeriodDetails_TestValidate_ReturnsValidResult()
    {
        RP14AEmployeePayDetailsArrearsOfPayPeriod model = new()
        {
            AOPOwed = 100.23M,
            Period = new PeriodType
            {
                StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
                EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            }
        };
        
        TestValidationResult<RP14AEmployeePayDetailsArrearsOfPayPeriod>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}