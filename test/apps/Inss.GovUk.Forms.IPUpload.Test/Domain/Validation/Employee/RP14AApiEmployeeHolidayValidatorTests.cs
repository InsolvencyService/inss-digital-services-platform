using System.Globalization;
using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class RP14AApiEmployeeHolidayValidatorTests
{
    private readonly RP14AApiEmployeeHolidayValidator _validator = new();
    
    [Fact]
    public void InvalidPeriodDates_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployeeHoliday model = new()
        {
            HolidayNotPaid =
            [
                new PeriodType
                {
                    StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
                    EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
                }
            ]
        };
        
        TestValidationResult<RP14AEmployeeHoliday>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidPeriodDetails_TestValidate_ReturnsValidResult()
    {
        RP14AEmployeeHoliday model = new()
        {
            HolidayNotPaid =
            [
                new PeriodType
                {
                    StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                    EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                }
            ]
        };
        
        TestValidationResult<RP14AEmployeeHoliday>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}