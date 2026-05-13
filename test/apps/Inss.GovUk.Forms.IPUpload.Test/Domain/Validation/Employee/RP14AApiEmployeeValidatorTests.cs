using System.Globalization;
using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class RP14AApiEmployeeValidatorTests
{
    private readonly RP14AApiEmployeeValidator _validator = new();
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void InvalidEmployeeSurname_TestValidate_ReturnsInvalidResult(string? invalidSurname)
    {
        RP14AEmployee model = new()
        {
            EmployeeName = new NameType { Surname = invalidSurname },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails { BasicPayPerWeek = 200.00M },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    }
                ]
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidEmploymentDates_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployee model = new()
        {
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails { BasicPayPerWeek = 200.00M },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    }
                ]
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidEmployeePay_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployee model = new()
        {
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails { BasicPayPerWeek = 200.123M },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    }
                ]
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidMoneyOwed_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployee model = new()
        {
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.123M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails { BasicPayPerWeek = 200.00M },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    }
                ]
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidHolidayRange_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployee model = new()
        {
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails { BasicPayPerWeek = 200.00M },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 366,
                HolidayDaysCarriedForward = 366,
                HolidayDaysTaken = 366,
                NoDaysHolidayOwed = 366,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    }
                ]
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }
    
    [Fact]
    public void InvalidHolidayNotPaidDates_TestValidate_ReturnsInvalidResult()
    {
        RP14AEmployee model = new()
        {
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails { BasicPayPerWeek = 200.00M },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
                        EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
                    }
                ]
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidEmployeeDetails_TestValidate_ReturnsValidResult()
    {
        RP14AEmployee model = new()
        {
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails { BasicPayPerWeek = 200.00M },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    }
                ]
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}