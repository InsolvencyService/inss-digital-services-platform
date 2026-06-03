using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

internal sealed class EmployeeSpreadsheetValidator : EmployeeValidator
{
    private readonly ICaseReferenceService _caseReferenceService;

    internal EmployeeSpreadsheetValidator(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }
    
    internal async Task<ValidatorContext> ValidateAsync(RP14A model)
    {
        EmployeeValidatorContext context = new();

        foreach (RP14AEmployee employee in model.Employee)
        {
            context.Forenames = employee.EmployeeName.Forenames;
            context.Surname = employee.EmployeeName.Surname;
            context.Dob = DateOnly.FromDateTime(employee.DateOfBirth);
            context.Nino = employee.NINO;
            
            bool caseReferenceExists = await _caseReferenceService.CheckExistsAsync(employee.Header.CaseReference);

            if (!caseReferenceExists)
            {
                context.AddError(CaseValidationInfo.UnknownCaseReference(), employee.Header.CaseReference);
            }

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