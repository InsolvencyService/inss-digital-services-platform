namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;

internal sealed class EmployeeLookups : Dictionary<string, ErrorInfoHeader>
{
    internal EmployeeLookups()
    {
        this["MissingEmployeeSurname"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Employee surname", Error = "[COUNT] missing employee surname"
        };
        this["InvalidLengthEmployeeSurname"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Employee surname", Error = "[COUNT] invalid length of the employee surname", Hint = "Maximum of 99 characters allowed"
        };
        
        this["InvalidFormatEmployeeAOP"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Arrears of payment owed", Error = "[COUNT] invalid arrears of pay owed", Hint = "Expected format is 12.34 or 100"
        };
        
        this["MissingEmployeeNino"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Employee national insurance number", Error = "[COUNT] missing the employee national insurance number"
        };
        this["InvalidFormatEmployeeNino"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Employee national insurance number", Error = "[COUNT] invalid employee national insurance number format", Hint = "Format is CN12345678"
        };
        
        this["InvalidFormatMoneyOwedToEmployer"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Money owed to employer", Error = "[COUNT] invalid money owed to employer", Hint = "Expected format is 12.34 or 100"
        };
        
        this["EmploymentEndBeforeStartDate"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Employee employment dates", Error = "[COUNT] invalid employment dates for the employee", Hint = "Start date must be before the end date"
        };
        
        this["AOPEndBeforeStartDate"] = new ErrorInfoHeader
        {
            Category = "Employee", Property = "Arrears of payment dates", Error = "[COUNT] invalid arrears of dates", Hint = "Start date must be before the end date"
        };
    }
}