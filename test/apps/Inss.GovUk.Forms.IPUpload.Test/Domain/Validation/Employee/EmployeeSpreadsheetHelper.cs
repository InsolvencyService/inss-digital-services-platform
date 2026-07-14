using System.Globalization;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

internal static class EmployeeSpreadsheetHelper
{
    internal static RP14A CreateModel()
    {
        return new RP14A
        {
            Employee =
            [
                new RP14AEmployee
                {
                    Header = new RP14AEmployeeHeader
                    {
                        CaseReference = "CN12345678"
                    },
                    EmployerName = "Springfield Nuclear",
                    EmployeeName = new NameType { Forenames = "Homer", Surname = "Simpson" },
                    NINO = "AB123456C",
                    MoneyOwedToEmployer = 1000.00M,
                    StartDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture),
                    EndDate = DateTime.Parse("2026-01-01", CultureInfo.InvariantCulture),
                    PayDetails = new RP14AEmployeePayDetails
                    {
                        BasicPayPerWeek = 200.00M,
                        ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                        {
                            ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1
                            {
                                AOPOwed1 = 100.00M,
                                AOP1StartDate = DateTime.Parse("2026-01-01", CultureInfo.InvariantCulture),
                                AOP1EndDate = DateTime.Parse("2026-01-10", CultureInfo.InvariantCulture)
                            },
                            ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2(),
                            ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3(),
                            ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4()
                        }
                    },
                    Holiday = new RP14AEmployeeHoliday
                    {
                        HolidayContractedEntitlementDays = 33,
                        HolidayDaysCarriedForward = 5,
                        HolidayDaysTaken = 29,
                        NoDaysHolidayOwed = 1,
                        HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                        {
                            Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                            {
                                Holiday1StartDate = DateTime.Parse("2025-01-01", CultureInfo.InvariantCulture),
                                Holiday1EndDate = DateTime.Parse("2026-01-01", CultureInfo.InvariantCulture)
                            },
                            Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2(),
                            Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3()
                        }
                    }
                }
            ]
        };
    }
    
    internal static void AssertError(List<Error> errors, RP14AEmployee employee, string? value, ValidationInfo validationInfo)
    {
        EmployeeError? error = errors.Cast<EmployeeError>().FirstOrDefault(
            e => e.Forenames == employee.EmployeeName.Forenames &&
                 e.Surname == employee.EmployeeName.Surname &&
                 e.Dob == DateOnly.FromDateTime(employee.DateOfBirth) &&
                 e.Nino == employee.NINO &&
                 e.Value == value &&
                 e.Info.Key == validationInfo.Key);
        Assert.NotNull(error);
    }
}