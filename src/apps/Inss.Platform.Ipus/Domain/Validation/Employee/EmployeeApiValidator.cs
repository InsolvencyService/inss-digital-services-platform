using Inss.Common.IPUpload.Employee.Api;

namespace Inss.Platform.Ipus.Domain.Validation.Employee;

public sealed class EmployeeApiValidator : EmployeeValidator
{
    private readonly RP14A _model;

    public EmployeeApiValidator(RP14A model)
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
                ValidateCaseReference(context, _model.Header.CaseReference, caseReference);
                validateCaseReference = false;
            }

            ValidateAverageHoursWorked(context, employee.AverageHoursWorked);
            ValidateEmployerName(context, _model.EmployerName);
            ValidateEmployeeSurname(context, employee.EmployeeName?.Surname ?? null!);
            ValidateEmployeeNino(context, employee.NINO);
            ValidateMoneyOwedToEmployer(context, employee.MoneyOwedToEmployer);
            ValidateEmploymentDates(context, employee.StartDate, employee.EndDate);

            if (employee.PayDetails is not null)
            {
                ValidateBasicPay(context, employee.PayDetails.BasicPayPerWeek);

                foreach (var arrearsOfPay in employee.PayDetails.ArrearsOfPay ?? [])
                {
                    ValidateArrearsOfPayOwed(context, arrearsOfPay.AOPOwed);
                    ValidateArrearsOfPayDates(context, arrearsOfPay.Period.StartDate, arrearsOfPay.Period.EndDate);
                }
            }

            if (employee.Holiday is not null)
            {
                ValidateHolidayEntitlement(context, employee.Holiday.HolidayContractedEntitlementDays);
                ValidateHolidayCarriedForward(context, employee.Holiday.HolidayDaysCarriedForward);
                ValidateHolidayDaysTaken(context, employee.Holiday.HolidayDaysTaken);
                ValidateHolidayDaysOwed(context, employee.Holiday.NoDaysHolidayOwed);

                foreach (var holidayNotPaid in employee.Holiday.HolidayNotPaid ?? [])
                {
                    ValidateHolidayNotPaidDates(context, holidayNotPaid.StartDate, holidayNotPaid.EndDate);
                }
            }
        }
        
        return context;
    }
}