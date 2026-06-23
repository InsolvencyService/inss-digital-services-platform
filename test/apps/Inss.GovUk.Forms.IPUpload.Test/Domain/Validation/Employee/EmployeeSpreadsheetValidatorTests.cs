using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using System.Globalization;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class EmployeeSpreadsheetValidatorTests
{
    private readonly EmployeeSpreadsheetValidator _validator;
    private readonly EmployerDetailsModel _employerDetails;
    private readonly RP14A _model;

    public EmployeeSpreadsheetValidatorTests()
    {
        _model = EmployeeSpreadsheetHelper.CreateModel();
        _employerDetails = new EmployerDetailsModel
        {
            CaseReference = _model.Employee[0].Header.CaseReference,
            EmployerName = "Test Company"
        };
        _validator = new EmployeeSpreadsheetValidator(_model);
    }

    [Fact]
    public void MismatchCaseRef_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Header.CaseReference = "CN87654321";
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            employee.Header.CaseReference, 
            CaseValidationInfo.CaseReferenceMismatch(employee.Header.CaseReference));
    }

    [Fact]
    public void InvalidAverageHoursWorked_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.AverageHoursWorked = 37.55M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.AverageHoursWorked:G}", 
            EmployeeValidationInfo.InvalidAverageHoursWorkedFormat());
    }
    
    [Fact]
    public void InvalidEmployerName_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.EmployerName = new string('X', 100);
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            employee.EmployerName, 
            EmployerValidationInfo.InvalidEmployerNameLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MissingEmployeeSurname_ValidateAsync_ReturnsError(string? surname)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.EmployeeName.Surname = surname;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            employee.EmployeeName.Surname, 
            EmployeeValidationInfo.MissingEmployeeSurname());
    }
    
    [Fact]
    public void InvalidEmployeeSurname_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.EmployeeName.Surname = new string('X', 100);
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            employee.EmployeeName.Surname, 
            EmployeeValidationInfo.InvalidEmployeeSurnameLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MissingEmployeeNino_ValidateAsync_ReturnsError(string? nino)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.NINO = nino;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
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
    public void InvalidEmployeeNino_ValidateAsync_ReturnsError(string? nino)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.NINO = nino;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            employee.NINO, 
            EmployeeValidationInfo.InvalidEmployeeNinoFormat());
    }
    
    [Fact]
    public void InvalidMoneyOwedToEmployer_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.MoneyOwedToEmployer = 300.123M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.MoneyOwedToEmployer:G}", 
            EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat());
    }
    
    [Fact]
    public void InvalidEmploymentDates_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.StartDate = DateTime.Parse("2025-01-30", CultureInfo.InvariantCulture);
        employee.EndDate = DateTime.Parse("2024-01-30", CultureInfo.InvariantCulture);
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.StartDate:d}, {employee.EndDate:d}", 
            EmployeeValidationInfo.InvalidEmployeeEmploymentDates());
    }
    
    [Fact]
    public void InvalidEmployeeBasicPay_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.PayDetails.BasicPayPerWeek = 250.123M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.PayDetails.BasicPayPerWeek:G}", 
            EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat());
    }
    
    [Fact]
    public void InvalidAOPOwed_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1 aop = employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1;
        aop.AOPOwed1 = 250.123M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{aop.AOPOwed1:G}", 
            EmployeeValidationInfo.InvalidAopOwedFormat());
    }
    
    [Fact]
    public void InvalidAOPDates_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1 aop = employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1;
        aop.AOP1StartDate = DateTime.Parse("2025-01-30", CultureInfo.InvariantCulture);
        aop.AOP1EndDate = DateTime.Parse("2024-01-30", CultureInfo.InvariantCulture);
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{aop.AOP1StartDate:d}, {aop.AOP1EndDate:d}", 
            EmployeeValidationInfo.InvalidEmployeeAopDates());
    }
    
    [Fact]
    public void InvalidHolidayEntitlement_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayContractedEntitlementDays = 33.555M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayContractedEntitlementDays:G}", 
            EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public void InvalidHolidayEntitlementRange_ValidateAsync_ReturnsError(decimal entitlement)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayContractedEntitlementDays = entitlement;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayContractedEntitlementDays:G}", 
            EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementRange());
    }
    
    [Fact]
    public void InvalidHolidayCarriedForward_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysCarriedForward = 33.555M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysCarriedForward:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public void InvalidHolidayCarriedForwardRange_ValidateAsync_ReturnsError(decimal carriedForward)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysCarriedForward = carriedForward;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysCarriedForward:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardRange());
    }
    
    [Fact]
    public void InvalidHolidayDaysTaken_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysTaken = 33.555M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysTaken:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public void InvalidHolidayDaysTakenRange_ValidateAsync_ReturnsError(decimal daysTaken)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.HolidayDaysTaken = daysTaken;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.HolidayDaysTaken:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenRange());
    }
    
    [Fact]
    public void InvalidHolidayDaysOwed_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.NoDaysHolidayOwed = 33.555M;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.NoDaysHolidayOwed:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayOwedFormat());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public void InvalidHolidayDaysOwedRange_ValidateAsync_ReturnsError(decimal daysOwed)
    {
        RP14AEmployee employee = _model.Employee[0];
        employee.Holiday.NoDaysHolidayOwed = daysOwed;
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{employee.Holiday.NoDaysHolidayOwed:G}", 
            EmployeeHolidayValidationInfo.InvalidHolidayOwedRange());
    }
    
    [Fact]
    public void InvalidHolidayNotPaidDates_ValidateAsync_ReturnsError()
    {
        RP14AEmployee employee = _model.Employee[0];
        RP14AEmployeeHolidayHolidayNotPaidHoliday1 holidayNotPaid = employee.Holiday.HolidayNotPaid.Holiday1;
        holidayNotPaid.Holiday1StartDate = DateTime.Parse("2025-01-30", CultureInfo.InvariantCulture);
        holidayNotPaid.Holiday1EndDate = DateTime.Parse("2024-01-30", CultureInfo.InvariantCulture);
        
        ValidatorContext context = _validator.Validate(_employerDetails);

        EmployeeSpreadsheetHelper.AssertError(
            context.Errors, 
            employee, 
            $"{holidayNotPaid.Holiday1StartDate:d}, {holidayNotPaid.Holiday1EndDate:d}", 
            EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange());
    }
    
    [Fact]
    public void ValidModel_ValidateAsync_ReturnsNoErrors()
    {
        ValidatorContext context = _validator.Validate(_employerDetails);

        Assert.NotNull(context.Errors);
    }
}