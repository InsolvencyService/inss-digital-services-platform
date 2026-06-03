using Inss.Common.IPUpload.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

internal sealed class EmployeeApiValidator : EmployeeValidator
{
    private readonly ICaseReferenceService _caseReferenceService;

    internal EmployeeApiValidator(ICaseReferenceService caseReferenceService)
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

            bool caseReferenceExists = await _caseReferenceService.CheckExistsAsync(model.Header.CaseReference);

            if (!caseReferenceExists)
            {
                context.AddError(CaseValidationInfo.UnknownCaseReference(), model.Header.CaseReference);
            }
            
            ValidateEmployerName(context, model.EmployerName);
            ValidateEmployeeSurname(context, employee.EmployeeName.Surname);
            ValidateEmployeeNino(context, employee.NINO);
            ValidateMoneyOwedToEmployer(context, employee.MoneyOwedToEmployer);
            ValidateEmploymentDates(context, employee.StartDate, employee.EndDate);

            ValidateBasicPay(context, employee.PayDetails.BasicPayPerWeek);

            foreach (var arrearsOfPay in employee.PayDetails.ArrearsOfPay)
            {
                ValidateArrearsOfPayOwed(context, arrearsOfPay.AOPOwed);
                ValidateArrearsOfPayDates(context, arrearsOfPay.Period.StartDate, arrearsOfPay.Period.EndDate);
            }

            ValidateHolidayEntitlement(context, employee.Holiday.HolidayContractedEntitlementDays);
            ValidateHolidayCarriedForward(context, employee.Holiday.HolidayDaysCarriedForward);
            ValidateHolidayDaysTaken(context, employee.Holiday.HolidayDaysTaken);
            ValidateHolidayDaysOwed(context, employee.Holiday.NoDaysHolidayOwed);

            foreach (var holidayNotPaid in employee.Holiday.HolidayNotPaid)
            {
                ValidateHolidayNotPaidDates(context, holidayNotPaid.StartDate, holidayNotPaid.EndDate);
            }
        }
        
        return context;
    }
}