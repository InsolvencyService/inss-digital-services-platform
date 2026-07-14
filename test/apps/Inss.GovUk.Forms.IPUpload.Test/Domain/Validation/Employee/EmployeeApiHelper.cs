using System.Globalization;
using Inss.Common.IPUpload.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

internal static class EmployeeApiHelper
{
    internal static RP14A CreateModel()
    {
        return new RP14A
        {
            Header = new RP14AHeader
            {
                CaseReference = "CN12345678",
            },
            EmployerName = "Springfield Nuclear",
            Employee =
            [
                new RP14AEmployee
                {
                    EmployeeName = new NameType { Forenames = "Homer", Surname = "Simpson" },
                    NINO = "AB123456C",
                    MoneyOwedToEmployer = 1000.00M,
                    StartDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture),
                    EndDate = DateTime.Parse("2026-01-01", CultureInfo.InvariantCulture),
                    PayDetails = new RP14AEmployeePayDetails
                    {
                        BasicPayPerWeek = 200.00M,
                        ArrearsOfPay = [
                            new RP14AEmployeePayDetailsArrearsOfPayPeriod
                            {
                                AOPOwed = 100.00M,
                                Period = new PeriodType
                                {
                                    StartDate = DateTime.Parse("2026-01-01", CultureInfo.InvariantCulture),
                                    EndDate = DateTime.Parse("2026-01-10", CultureInfo.InvariantCulture)   
                                }
                            }
                        ]
                    },
                    Holiday = new RP14AEmployeeHoliday
                    {
                        HolidayContractedEntitlementDays = 33,
                        HolidayDaysCarriedForward = 5,
                        HolidayDaysTaken = 29,
                        NoDaysHolidayOwed = 1,
                        HolidayNotPaid = [
                            new PeriodType
                            {
                                StartDate = DateTime.Parse("2025-01-01", CultureInfo.InvariantCulture),
                                EndDate = DateTime.Parse("2026-01-01", CultureInfo.InvariantCulture)
                            }
                        ]
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