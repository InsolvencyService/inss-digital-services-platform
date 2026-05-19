using System.Globalization;
using FluentValidation.TestHelper;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class RP14ASpreadsheetEmployeeValidatorTests
{
    private readonly RP14ASpreadsheetEmployeeValidator _validator = new();
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void InvalidEmployeeSurname_TestValidate_ReturnsInvalidResult(string? invalidSurname)
    {
        RP14AEmployee model = new()
        {
            Header = new RP14AEmployeeHeader { CaseReference = "CN12345678" },
            EmployeeName = new NameType { Surname = invalidSurname },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = 200.00M,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1(),
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                }
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                }
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
            Header = new RP14AEmployeeHeader { CaseReference = "CN12345678" },
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = 200.00M,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1(),
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                }
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                }
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
            Header = new RP14AEmployeeHeader { CaseReference = "CN12345678" },
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = 200.123M,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1(),
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                }
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                }
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
            Header = new RP14AEmployeeHeader { CaseReference = "CN12345678" },
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.123M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = 200.00M,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1(),
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                }
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                }
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
            Header = new RP14AEmployeeHeader { CaseReference = "CN12345678" },
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = 200.00M,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1(),
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                }
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 366,
                HolidayDaysCarriedForward = 366,
                HolidayDaysTaken = 366,
                NoDaysHolidayOwed = 366,
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                }
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
            Header = new RP14AEmployeeHeader { CaseReference = "CN12345678" },
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = 200.00M,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1(),
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                }
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
                        Holiday1EndDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture)
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                }
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
            Header = new RP14AEmployeeHeader { CaseReference = "CN12345678" },
            EmployeeName = new NameType { Surname = "Simpson" },
            NINO = "AB123456C",
            MoneyOwedToEmployer = 1000.00M,
            StartDate = DateTime.Parse("2024-01-01", CultureInfo.CurrentCulture),
            EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture),
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = 200.00M,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1(),
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                }
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayContractedEntitlementDays = 30,
                HolidayDaysCarriedForward = 2,
                HolidayDaysTaken = 20,
                NoDaysHolidayOwed = 12,
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.CurrentCulture),
                        Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.CurrentCulture)
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                }
            }
        };
        
        TestValidationResult<RP14AEmployee>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}