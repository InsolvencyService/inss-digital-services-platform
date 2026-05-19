using System.Globalization;
using FluentValidation.TestHelper;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class RP14ASpreadsheetArrearsOfPayValidatorTests
{
    private readonly RP14ASpreadsheetArrearsOfPayValidator _validator = new();
    
    [Fact]
    public void InvalidArrearsOfPay_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployeePayDetailsArrearsOfPay model = new()
        {
           ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1
           {
               AOP1StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
               AOP1EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
           },
           ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2
           {
               AOP2StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
               AOP2EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
           },
           ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3
           {
               AOP3StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
               AOP3EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
           },
           ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4
           {
               AOP4StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
               AOP4EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
           }
        };
        
        TestValidationResult<RP14AEmployeePayDetailsArrearsOfPay>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }
    
    [Fact]
    public void ValidHeaderDetails_TestValidate_ReturnsValidResult()
    {
        RP14AEmployeePayDetailsArrearsOfPay model = new()
        {
            ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1
            {
                AOP1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                AOP1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            },
            ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2
            {
                AOP2StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                AOP2EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            },
            ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3
            {
                AOP3StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                AOP3EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            },
            ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4
            {
                AOP4StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                AOP4EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
            }
        };
        
        TestValidationResult<RP14AEmployeePayDetailsArrearsOfPay>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}