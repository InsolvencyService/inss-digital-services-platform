using Inss.Common.IPUpload.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

internal sealed class EmployeeApiValidator : EmployeeValidator
{
    private readonly RP14A _model;

    internal EmployeeApiValidator(RP14A model, ICaseReferenceService caseReferenceService) : base(caseReferenceService)
    {
        _model = model;
    }
    
    internal override async Task<ValidatorContext> ValidateAsync()
    {
        EmployeeValidatorContext context = new();

        await ValidateCaseReferenceAsync(context, _model.Header.CaseReference);
        
        foreach (RP14AEmployee employee in _model.Employee)
        {
            context.Forenames = employee.EmployeeName.Forenames;
            context.Surname = employee.EmployeeName.Surname;
            context.Dob = DateOnly.FromDateTime(employee.DateOfBirth);
            context.Nino = employee.NINO;
            
            ValidateAverageHoursWorked(context, employee.AverageHoursWorked);
            ValidateEmployerName(context, _model.EmployerName);
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