using Inss.Common.IPUpload.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using NSubstitute;
using System.Globalization;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class EmployeeApiValidatorTests
{
    private readonly EmployeeApiValidator _validator;
    private readonly ICaseReferenceService _caseReferenceService;
    private readonly RP14A _model;

    public EmployeeApiValidatorTests()
    {
        _caseReferenceService = Substitute.For<ICaseReferenceService>();
        _model = EmployeeApiHelper.CreateModel();
        _caseReferenceService
        .GetEmployerDetailsAsync(_model.Header.CaseReference)
        .Returns(new CaseDetailModel
        {
            CaseReference = _model.Header.CaseReference,
            CompanyName = "Test Company"
        });
        _validator = new EmployeeApiValidator(_model, _caseReferenceService);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingCaseRef_ValidateAsync_ReturnsError(string? caseRef)
    {
        RP14AEmployee employee = _model.Employee[0];
        _model.Header.CaseReference = caseRef;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            _model.Header.CaseReference, 
            CaseValidationInfo.MissingCaseReference());
    }
    
    [Fact]
    public async Task InvalidLengthCaseRef_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        _model.Header.CaseReference = "CN123456789";
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            _model.Header.CaseReference,
            CaseValidationInfo.InvalidCaseReferenceLength());
    }
    
    [Fact]
    public async Task UnknownCaseRef_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        _caseReferenceService
        .GetEmployerDetailsAsync(_model.Header.CaseReference)
        .Returns((CaseDetailModel?)null);

        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            _model.Header.CaseReference, 
            CaseValidationInfo.UnknownCaseReference());
    }
    
    [Fact]
    public async Task InvalidAverageHoursWorked_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.AverageHoursWorked = 37.55M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors,
            employee, 
            $"{employee.AverageHoursWorked:G}", 
            EmployeeValidationInfo.InvalidAverageHoursWorkedFormat());
    }
    
    [Fact]
    public async Task InvalidEmployerName_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        _model.EmployerName = new string('X', 100);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            _model.EmployerName, 
            EmployerValidationInfo.InvalidEmployerNameLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingEmployeeSurname_ValidateAsync_ReturnsError(string? surname)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.EmployeeName.Surname = surname;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            employee.EmployeeName.Surname, 
            EmployeeValidationInfo.MissingEmployeeSurname());
    }
    
    [Fact]
    public async Task InvalidEmployeeSurname_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.EmployeeName.Surname = new string('X', 100);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            employee.EmployeeName.Surname, 
            EmployeeValidationInfo.InvalidEmployeeSurnameLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingEmployeeNino_ValidateAsync_ReturnsError(string? nino)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.NINO = nino;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            employee.NINO, 
            EmployeeValidationInfo.MissingEmployeeNino());
    }
    
    [Theory]
    [InlineData("ABC 11 22 33 G")] // Preceding chars
    [InlineData("ABC112233G")] // Preceding chars - no spaces
    [InlineData("AB 11 22 33 GH")] // Trailing chars
    [InlineData("AB112233GH")] // Trailing chars - no spaces
    public async Task InvalidEmployeeNino_ValidateAsync_ReturnsError(string? nino)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.NINO = nino;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            employee.NINO, 
            EmployeeValidationInfo.InvalidEmployeeNinoFormat());
    }
    
    [Fact]
    public async Task InvalidMoneyOwedToEmployer_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.MoneyOwedToEmployer = 300.123M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.MoneyOwedToEmployer:G}", 
            EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat());
    }
    
    [Fact]
    public async Task InvalidEmploymentDates_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.StartDate = DateTime.Parse("2025-01-30", CultureInfo.InvariantCulture);
        employee.EndDate = DateTime.Parse("2024-01-30", CultureInfo.InvariantCulture);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.StartDate:d}, {employee.EndDate:d}", 
            EmployeeValidationInfo.InvalidEmployeeEmploymentDates());
    }
    
    [Fact]
    public async Task InvalidEmployeeBasicPay_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.PayDetails.BasicPayPerWeek = 250.123M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.PayDetails.BasicPayPerWeek:G}", 
            EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat());
    }
    
    [Fact]
    public async Task InvalidAOPOwed_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        RP14AEmployeePayDetailsArrearsOfPayPeriod aop = employee.PayDetails.ArrearsOfPay[0];
        aop.AOPOwed = 250.123M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{aop.AOPOwed:G}", 
            EmployeeValidationInfo.InvalidAopOwedFormat());
    }
    
    [Fact]
    public async Task InvalidAOPDates_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        RP14AEmployeePayDetailsArrearsOfPayPeriod aop = employee.PayDetails.ArrearsOfPay[0];
        aop.Period.StartDate = DateTime.Parse("2025-01-30", CultureInfo.InvariantCulture);
        aop.Period.EndDate = DateTime.Parse("2024-01-30", CultureInfo.InvariantCulture);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{aop.Period.StartDate:d}, {aop.Period.EndDate:d}", 
            EmployeeValidationInfo.InvalidEmployeeAopDates());
    }
    
    [Fact]
    public async Task InvalidHolidayEntitlement_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayContractedEntitlementDays = 33.555M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayContractedEntitlementDays:G}", 
            EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public async Task InvalidHolidayEntitlementRange_ValidateAsync_ReturnsError(decimal entitlement)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayContractedEntitlementDays = entitlement;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayContractedEntitlementDays:G}", 
            EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementRange());
    }
    
    [Fact]
    public async Task InvalidHolidayCarriedForward_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysCarriedForward = 33.555M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysCarriedForward:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public async Task InvalidHolidayCarriedForwardRange_ValidateAsync_ReturnsError(decimal carriedForward)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysCarriedForward = carriedForward;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysCarriedForward:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardRange());
    }
    
    [Fact]
    public async Task InvalidHolidayDaysTaken_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysTaken = 33.555M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysTaken:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public async Task InvalidHolidayDaysTakenRange_ValidateAsync_ReturnsError(decimal daysTaken)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysTaken = daysTaken;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysTaken:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenRange());
    }
    
    [Fact]
    public async Task InvalidHolidayDaysOwed_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.NoDaysHolidayOwed = 33.555M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.NoDaysHolidayOwed:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayOwedFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public async Task InvalidHolidayDaysOwedRange_ValidateAsync_ReturnsError(decimal daysOwed)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.NoDaysHolidayOwed = daysOwed;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.NoDaysHolidayOwed:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayOwedRange());
    }
    
    [Fact]
    public async Task InvalidHolidayNotPaidDates_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        PeriodType holidayNotPaid = employee.Holiday.HolidayNotPaid[0];
        holidayNotPaid.StartDate = DateTime.Parse("2025-01-30", CultureInfo.InvariantCulture);
        holidayNotPaid.EndDate = DateTime.Parse("2024-01-30", CultureInfo.InvariantCulture);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployeeApiHelper.AssertError(
            context.Errors, 
            employee, 
            $"{holidayNotPaid.StartDate:d}, {holidayNotPaid.EndDate:d}", 
            EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange());
    }
    
    [Fact]
    public async Task ValidModel_ValidateAsync_ReturnsNoErrors()
    {
        ValidatorContext context = await _validator.ValidateAsync();

        Assert.NotNull(context.Errors);
    }
}