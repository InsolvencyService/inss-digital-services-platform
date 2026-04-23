using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Lookups;

public class ValidationAnnotationProviderTests
{
    [Fact]
    public void UnknownCaseReference_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("UnknownCaseReference");

        AssertErrorInfo(errorInfo, "Case", "Case reference", "[COUNT] case reference have not been matched in our system");
    }

    [Fact]
    public void MissingCaseReference_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("MissingCaseReference");

        AssertErrorInfo(errorInfo, "Case", "Case reference", "[COUNT] missing a case reference");
    }
    
    [Fact]
    public void InvalidFormatCaseReference_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidFormatCaseReference");

        AssertErrorInfo(errorInfo, "Case", "Case reference", "[COUNT] invalid case reference format", "Format is CN12345678");
    }
    
    [Fact]
    public void InvalidLengthCaseReference_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidLengthCaseReference");

        AssertErrorInfo(errorInfo, "Case", "Case reference", "[COUNT] too long case reference", "Up to 12 characters are allowed");
    }
    
    [Fact]
    public void InvalidLengthEmployerName_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidLengthEmployerName");

        AssertErrorInfo(errorInfo, "Employer", "Employer name", "[COUNT] invalid length of the employer name", "Maximum of 99 characters allowed");
    }
    
    [Fact]
    public void MissingEmployeeSurname_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("MissingEmployeeSurname");

        AssertErrorInfo(errorInfo, "Employee", "Employee surname", "[COUNT] missing employee surname");
    }
    
    [Fact]
    public void InvalidLengthEmployeeSurname_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidLengthEmployeeSurname");

        AssertErrorInfo(errorInfo, "Employee", "Employee surname", "[COUNT] invalid length of the employee surname", "Maximum of 99 characters allowed");
    }
    
    [Fact]
    public void InvalidFormatEmployeeAOP_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidFormatEmployeeAOP");

        AssertErrorInfo(errorInfo, "Employee", "Employee arrears of payment owed", "[COUNT] invalid arrears of pay owed", "Expected format is 12.34 or 100");
    }
    
    [Fact]
    public void MissingEmployeeNino_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("MissingEmployeeNino");

        AssertErrorInfo(errorInfo, "Employee", "Employee national insurance number", "[COUNT] missing the employee national insurance number");
    }
    
    [Fact]
    public void InvalidFormatEmployeeNino_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidFormatEmployeeNino");

        AssertErrorInfo(errorInfo, "Employee", "Employee national insurance number", "[COUNT] invalid employee national insurance number format", "Format is CN12345678");
    }
    
    [Fact]
    public void InvalidFormatMoneyOwedToEmployer_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidFormatMoneyOwedToEmployer");

        AssertErrorInfo(errorInfo, "Employee", "Money owed to employer", "[COUNT] invalid money owed to employer", "Expected format is 12.34 or 100");
    }
    
    [Fact]
    public void EmploymentEndBeforeStartDate_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("EmploymentEndBeforeStartDate");

        AssertErrorInfo(errorInfo, "Employee", "Employee employment dates", "[COUNT] invalid employment dates for the employee", "Start date must be before the end date");
    }
    
    [Fact]
    public void AOPEndBeforeStartDate_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("AOPEndBeforeStartDate");

        AssertErrorInfo(errorInfo, "Employee", "Employee arrears of payment dates", "[COUNT] invalid arrears of dates", "Start date must be before the end date");
    }
    
    [Fact]
    public void InvalidFormatEmployeeBasicPayPerWeek_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidFormatEmployeeBasicPayPerWeek");

        AssertErrorInfo(errorInfo, "Employee pay", "Employee basic pay per week", "[COUNT] invalid basic pay per week", "Expected format is 12.34 or 100");
    }
    
    [Fact]
    public void MissingContractedHolidayEntitlement_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("MissingContractedHolidayEntitlement");

        AssertErrorInfo(errorInfo, "Employee holiday", "Contracted holiday entitlement", "[COUNT] missing contracted holiday entitlement");
    }
    
    [Fact]
    public void InvalidContractedHolidayEntitlement_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidContractedHolidayEntitlement");

        AssertErrorInfo(errorInfo, "Employee holiday", "Contracted holiday entitlement", "[COUNT] invalid contracted holiday entitlement", "Expected format is 28.25 or 33");
    }
    
    [Fact]
    public void InvalidRangeContractedHolidayEntitlement_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidRangeContractedHolidayEntitlement");

        AssertErrorInfo(errorInfo, "Employee holiday", "Contracted holiday entitlement", "[COUNT] invalid range of contracted holiday entitlement", "0 to 365 days allowed");
    }
    
    [Fact]
    public void MissingHolidayCarriedForward_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("MissingHolidayCarriedForward");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday carried forward", "[COUNT] missing holiday days carried forward");
    }
    
    [Fact]
    public void InvalidHolidayCarriedForward_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidHolidayCarriedForward");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday carried forward", "[COUNT] invalid holiday days carried forward", "Expected format is 28.25 or 33");
    }
    
    [Fact]
    public void InvalidRangeHolidayCarriedForward_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidRangeHolidayCarriedForward");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday carried forward", "[COUNT] invalid range of holiday days carried forward", "0 to 365 days allowed");
    }
    
    [Fact]
    public void MissingHolidayTaken_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("MissingHolidayTaken");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday days taken", "[COUNT] missing holiday days taken");
    }
    
    [Fact]
    public void InvalidHolidayTaken_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidHolidayTaken");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday days taken", "[COUNT] invalid holiday days taken", "Expected format is 28.25 or 33");
    }
    
    [Fact]
    public void InvalidRangeHolidayTaken_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidRangeHolidayTaken");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday days taken", "[COUNT] invalid range of holiday days taken", "0 to 365 days allowed");
    }
    
    [Fact]
    public void MissingHolidayOwed_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("MissingHolidayOwed");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday owed", "[COUNT] missing holiday owed");
    }
    
    [Fact]
    public void InvalidHolidayOwed_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidHolidayOwed");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday owed", "[COUNT] invalid holiday owed", "Expected format is 28.25 or 33");
    }
    
    [Fact]
    public void InvalidRangeHolidayOwed_GetErrorInfo_ReturnsErrorInfo()
    {
        ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo("InvalidRangeHolidayOwed");

        AssertErrorInfo(errorInfo, "Employee holiday", "Holiday owed", "[COUNT] invalid range of holiday owed", "0 to 365 days allowed");
    }
    
    private static void AssertErrorInfo(ErrorInfo errorInfo, string category, string property, string errorTemplate, string? hint = null)
    {
        Assert.Equal(category, errorInfo.Category);
        Assert.Equal(property, errorInfo.Property);
        Assert.Equal(errorTemplate, errorInfo.Error);
        Assert.Equal(hint, errorInfo.Hint);
    }
}