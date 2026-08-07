using Inss.Common.IPUpload.Employee.Spreadsheet;

namespace Inss.Platform.Ipus.Domain.Validation.Employee;

public sealed class EmployeeSpreadsheetValidator : EmployeeValidator
{
    private readonly RP14A _model;

    public EmployeeSpreadsheetValidator(RP14A model)
    {
        _model = model;
    }

    public override ValidatorContext Validate(string caseReference)
    {
        EmployeeValidatorContext context = new();
        bool validateCaseReference = true;

        foreach (RP14AEmployee employee in _model.Employee)
        {
            context.Forenames = employee.EmployeeName?.Forenames ?? null!;
            context.Surname = employee.EmployeeName?.Surname ?? null!;
            context.Dob = DateOnly.FromDateTime(employee.DateOfBirth);
            context.Nino = employee.NINO;

            // The instructions on the spreadsheet state to define the case ref in the first row - this reflects it in validation
            if (validateCaseReference)
            {
                ValidateCaseReference(context, employee.Header.CaseReference, caseReference);
                validateCaseReference = false;
            }

            ValidateAverageHoursWorked(context, employee.AverageHoursWorked);
            ValidateEmployerName(context, employee.EmployerName);
            ValidateEmployeeSurname(context, employee.EmployeeName?.Surname ?? null!);
            ValidateEmployeeNino(context, employee.NINO);
            ValidateMoneyOwedToEmployer(context, employee.MoneyOwedToEmployer);
            ValidateEmploymentDates(context, employee.StartDate, employee.EndDate);

            if (employee.PayDetails is not null)
            {
                ValidateBasicPay(context, employee.PayDetails.BasicPayPerWeek);
            }

            if (employee.PayDetails?.ArrearsOfPay is not null)
            {
                RP14AEmployeePayDetailsArrearsOfPay arrearsOfPay = employee.PayDetails.ArrearsOfPay;
                ValidateArrearsOfPay(context, arrearsOfPay);
            }

            if (employee.Holiday is not null)
            {
                ValidateHoliday(context, employee.Holiday);

                if (employee.Holiday.HolidayNotPaid is not null)
                {
                    ValidateHolidayNotPaid(context, employee.Holiday.HolidayNotPaid);
                }
            }
        }

        return context;
    }

    private static void ValidateArrearsOfPay(EmployeeValidatorContext context, RP14AEmployeePayDetailsArrearsOfPay arrearsOfPay)
    {
        if (arrearsOfPay.ArrearsOfPayPeriod1 is not null)
        {
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod1.AOPOwed1);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod1.AOP1StartDate, arrearsOfPay.ArrearsOfPayPeriod1.AOP1EndDate);
        }

        if (arrearsOfPay.ArrearsOfPayPeriod2 is not null)
        {
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod2.AOPOwed2);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod2.AOP2StartDate, arrearsOfPay.ArrearsOfPayPeriod2.AOP2EndDate);
        }

        if (arrearsOfPay.ArrearsOfPayPeriod3 is not null)
        {
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod3.AOPOwed3);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod3.AOP3StartDate, arrearsOfPay.ArrearsOfPayPeriod3.AOP3EndDate);
        }

        if (arrearsOfPay.ArrearsOfPayPeriod4 is not null)
        {
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod4.AOPOwed4);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod4.AOP4StartDate, arrearsOfPay.ArrearsOfPayPeriod4.AOP4EndDate);
        }
    }

    private static void ValidateHoliday(EmployeeValidatorContext context, RP14AEmployeeHoliday holiday)
    {
        ValidateHolidayEntitlement(context, holiday.HolidayContractedEntitlementDays);
        ValidateHolidayCarriedForward(context, holiday.HolidayDaysCarriedForward);
        ValidateHolidayDaysTaken(context, holiday.HolidayDaysTaken);
        ValidateHolidayDaysOwed(context, holiday.NoDaysHolidayOwed);
    }

    private static void ValidateHolidayNotPaid(EmployeeValidatorContext context, RP14AEmployeeHolidayHolidayNotPaid holidayNotPaid)
    {
        if (holidayNotPaid.Holiday1 is not null)
        {
            ValidateHolidayNotPaidDates(context, holidayNotPaid.Holiday1.Holiday1StartDate, holidayNotPaid.Holiday1.Holiday1EndDate);    
        }
        
        if (holidayNotPaid.Holiday2 is not null)
        {
            ValidateHolidayNotPaidDates(context, holidayNotPaid.Holiday2.Holiday2StartDate, holidayNotPaid.Holiday2.Holiday2EndDate);    
        }
        
        if (holidayNotPaid.Holiday3 is not null)
        {
            ValidateHolidayNotPaidDates(context, holidayNotPaid.Holiday3.Holiday3StartDate, holidayNotPaid.Holiday3.Holiday3EndDate);   
        }
    }
}