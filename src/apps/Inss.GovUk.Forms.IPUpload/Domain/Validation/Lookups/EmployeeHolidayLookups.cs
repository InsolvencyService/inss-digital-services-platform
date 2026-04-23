namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;

internal sealed class EmployeeHolidayLookups : Dictionary<string, ErrorInfoHeader>
{
    internal EmployeeHolidayLookups()
    {
        this["MissingContractedHolidayEntitlement"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Contracted holiday entitlement", Error = "[COUNT] missing contracted holiday entitlement"
        };
        this["InvalidContractedHolidayEntitlement"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Contracted holiday entitlement", Error = "[COUNT] invalid contracted holiday entitlement", Hint = "Expected format is 28.25 or 33"
        };
        this["InvalidRangeContractedHolidayEntitlement"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Contracted holiday entitlement", Error = "[COUNT] invalid range of contracted holiday entitlement", Hint = "0 to 365 days allowed"
        };
        
        this["MissingHolidayCarriedForward"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday carried forward", Error = "[COUNT] missing holiday days carried forward"
        };
        this["InvalidHolidayCarriedForward"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday carried forward", Error = "[COUNT] invalid holiday days carried forward", Hint = "Expected format is 28.25 or 33"
        };
        this["InvalidRangeHolidayCarriedForward"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday carried forward", Error = "[COUNT] invalid range of holiday days carried forward", Hint = "0 to 365 days allowed"
        };
        
        this["MissingHolidayTaken"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday days taken", Error = "[COUNT] missing holiday days taken"
        };
        this["InvalidHolidayTaken"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday days taken", Error = "[COUNT] invalid holiday days taken", Hint = "Expected format is 28.25 or 33"
        };
        this["InvalidRangeHolidayTaken"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday days taken", Error = "[COUNT] invalid range of holiday days taken", Hint = "0 to 365 days allowed"
        };
        
        this["MissingHolidayOwed"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday owed", Error = "[COUNT] missing holiday owed"
        };
        this["InvalidHolidayOwed"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday owed", Error = "[COUNT] invalid holiday owed", Hint = "Expected format is 28.25 or 33"
        };
        this["InvalidRangeHolidayOwed"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday owed", Error = "[COUNT] invalid range of holiday owed", Hint = "0 to 365 days allowed"
        };
        
        this["HolidayNotPaidEndBeforeStartDate"] = new ErrorInfoHeader
        {
            Category = "Employee holiday", Property = "Holiday not paid", Error = "[COUNT] invalid holiday not paid of dates", Hint = "Start date must be before the end date"
        };
    }
}