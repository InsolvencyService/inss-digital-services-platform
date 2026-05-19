using System.Globalization;
using FluentValidation.TestHelper;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class RP14ASpreadsheetHolidayNotPaidValidatorTests
{
    private readonly RP14ASpreadsheetHolidayNotPaidValidator _validator = new();
    
    [Fact]
    public void InvalidHolidayNotPaidDates_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployeeHolidayHolidayNotPaid model = new()
        {
            Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
            {
                Holiday1StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
                Holiday1EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
            },
            Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2
            {
                Holiday2StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
                Holiday2EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
            },
            Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3
            {
                Holiday3StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
                Holiday3EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
            }
        };
        
        TestValidationResult<RP14AEmployeeHolidayHolidayNotPaid>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
    }
    
    [Fact]
    public void ValidHolidayNotPaid_TestValidate_ReturnsValidResult()
    {
        RP14AEmployeeHolidayHolidayNotPaid model = new()
        {
            Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
            {
                Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            },
            Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2
            {
                Holiday2StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                Holiday2EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            },
            Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3
            {
                Holiday3StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                Holiday3EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            }
        };
        
        TestValidationResult<RP14AEmployeeHolidayHolidayNotPaid>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}