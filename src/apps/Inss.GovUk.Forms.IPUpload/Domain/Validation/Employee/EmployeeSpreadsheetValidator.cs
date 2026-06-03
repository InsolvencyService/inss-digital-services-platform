using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

internal sealed class EmployeeSpreadsheetValidator : EmployeeValidator
{
    private readonly RP14A _model;

    internal EmployeeSpreadsheetValidator(RP14A model, ICaseReferenceService caseReferenceService) : base(caseReferenceService)
    {
        _model = model;
    }
    
    internal override async Task<ValidatorContext> ValidateAsync()
    {
        EmployeeValidatorContext context = new();

        foreach (RP14AEmployee employee in _model.Employee)
        {
            context.Forenames = employee.EmployeeName.Forenames;
            context.Surname = employee.EmployeeName.Surname;
            context.Dob = DateOnly.FromDateTime(employee.DateOfBirth);
            context.Nino = employee.NINO;
            
            await ValidateCaseReferenceAsync(context, employee.Header.CaseReference);

            ValidateAverageHoursWorked(context, employee.AverageHoursWorked);
            ValidateEmployerName(context, employee.EmployerName);
            ValidateEmployeeSurname(context, employee.EmployeeName.Surname);
            ValidateEmployeeNino(context, employee.NINO);
            ValidateMoneyOwedToEmployer(context, employee.MoneyOwedToEmployer);
            ValidateEmploymentDates(context, employee.StartDate, employee.EndDate);

            ValidateBasicPay(context, employee.PayDetails.BasicPayPerWeek);

            RP14AEmployeePayDetailsArrearsOfPay arrearsOfPay = employee.PayDetails.ArrearsOfPay;
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod1.AOPOwed1);
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod2.AOPOwed2);
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod3.AOPOwed3);
            ValidateArrearsOfPayOwed(context, arrearsOfPay.ArrearsOfPayPeriod4.AOPOwed4);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod1.AOP1StartDate, arrearsOfPay.ArrearsOfPayPeriod1.AOP1EndDate);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod2.AOP2StartDate, arrearsOfPay.ArrearsOfPayPeriod2.AOP2EndDate);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod3.AOP3StartDate, arrearsOfPay.ArrearsOfPayPeriod3.AOP3EndDate);
            ValidateArrearsOfPayDates(context, arrearsOfPay.ArrearsOfPayPeriod4.AOP4StartDate, arrearsOfPay.ArrearsOfPayPeriod4.AOP4EndDate);

            ValidateHolidayEntitlement(context, employee.Holiday.HolidayContractedEntitlementDays);
            ValidateHolidayCarriedForward(context, employee.Holiday.HolidayDaysCarriedForward);
            ValidateHolidayDaysTaken(context, employee.Holiday.HolidayDaysTaken);
            ValidateHolidayDaysOwed(context, employee.Holiday.NoDaysHolidayOwed);

            RP14AEmployeeHolidayHolidayNotPaid holidayNotPaid = employee.Holiday.HolidayNotPaid;
            ValidateHolidayNotPaidDates(context, holidayNotPaid.Holiday1.Holiday1StartDate, holidayNotPaid.Holiday1.Holiday1EndDate);
            ValidateHolidayNotPaidDates(context, holidayNotPaid.Holiday2.Holiday2StartDate, holidayNotPaid.Holiday2.Holiday2EndDate);
            ValidateHolidayNotPaidDates(context, holidayNotPaid.Holiday3.Holiday3StartDate, holidayNotPaid.Holiday3.Holiday3EndDate);
        }
        
        return context;
    }
}