using Inss.Common.IPUpload.Employer.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class EmployerSpreadsheetValidatorTests
{
    private readonly EmployerSpreadsheetValidator _validator;
    private readonly ICaseReferenceService _caseReferenceService;
    private readonly RP14 _model;

    public EmployerSpreadsheetValidatorTests()
    {
        _caseReferenceService = Substitute.For<ICaseReferenceService>();
        _model = EmployerSpreadsheetHelper.CreateModel();
        _caseReferenceService
        .GetCaseDetailsAsync(_model.Header.CaseReference)
        .Returns(new CaseDetailModel
        {
            CaseReference = _model.Header.CaseReference,
            CompanyName = "Test Company"
        });
        _validator = new EmployerSpreadsheetValidator(_model, _caseReferenceService);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingCaseRef_ValidateAsync_ReturnsError(string? caseRef)
    {
        _model.Header.CaseReference = caseRef;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, CaseValidationInfo.MissingCaseReference());
    }
    
    [Fact]
    public async Task InvalidLengthCaseRef_ValidateAsync_ReturnsError()
    {
        _model.Header.CaseReference = "CN123456789";
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, CaseValidationInfo.InvalidCaseReferenceLength());
    }
    
    [Fact]
    public async Task UnknownCaseRef_ValidateAsync_ReturnsError()
    {
        _caseReferenceService
         .GetCaseDetailsAsync(_model.Header.CaseReference)
         .Returns((CaseDetailModel?)null);

        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, CaseValidationInfo.UnknownCaseReference());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingBusinessName_ValidateAsync_ReturnsError(string? businessName)
    {
        _model.NameOfBusiness = businessName;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, BusinessValidationInfo.MissingBusinessName());
    }
    
    [Fact]
    public async Task InvalidBusinessName_ValidateAsync_ReturnsError()
    {
        _model.NameOfBusiness = new string('X', 61);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, BusinessValidationInfo.InvalidBusinessNameLength());
    }
    
    [Fact]
    public async Task InvalidCompanyNumber_ValidateAsync_ReturnsError()
    {
        _model.CompanyNumber = new string('X', 13);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, BusinessValidationInfo.InvalidCompanyNumberLength());
    }
    
    [Fact]
    public async Task InvalidNatureOfBusiness_ValidateAsync_ReturnsError()
    {
        _model.NatureOfBusiness = new string('X', 101);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, BusinessValidationInfo.InvalidNatureOfBusinessLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingBusinessAddressLine1_ValidateAsync_ReturnsError(string? line1)
    {
        _model.CompanyAddrLine1 = line1;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.MissingAddressLine1("Business"));
    }
    
    [Fact]
    public async Task InvalidBusinessAddress_ValidateAsync_ReturnsError()
    {
        _model.CompanyAddrLine1 = new string('X', 36);
        _model.CompanyAddrLine2 = new string('X', 36);
        _model.CompanyAddrTown = new string('X', 36);
        _model.CompanyAddrCounty = new string('X', 36);
        _model.CompanyAddrPostcode = new string('X', 11);
        _model.CompanyAddrCountry = new string('X', 11);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Business"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Business"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressTownLength("Business"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountyLength("Business"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressPostcodeLength("Business"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountryLength("Business"));
    }
    
    [Fact]
    public async Task InvalidBusinessSICCode_ValidateAsync_ReturnsError()
    {
        _model.SICCode = new string('X', 256);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, BusinessValidationInfo.InvalidSICLength());
    }
    
    [Fact]
    public async Task InvalidDirectorInitials_ValidateAsync_ReturnsError()
    {
        _model.Directors.Director1.Director1Initials = new string('X', 101);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, DirectorValidationInfo.InvalidDirectorInitialsLength());
    }
    
    [Fact]
    public async Task InvalidDirectorSurname_ValidateAsync_ReturnsError()
    {
        _model.Directors.Director1.Director1Surname = new string('X', 101);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, DirectorValidationInfo.InvalidDirectorSurnameLength());
    }
    
    [Theory]
    [InlineData("ABC 11 22 33 G")] // Preceding chars
    [InlineData("ABC112233G")] // Preceding chars - no spaces
    [InlineData("AB 11 22 33 GH")] // Trailing chars
    [InlineData("AB112233GH")] // Trailing chars - no spaces
    public async Task InvalidDirectorNino_ValidateAsync_ReturnsError(string nino)
    {
        _model.Directors.Director1.Director1NINO = nino;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, DirectorValidationInfo.InvalidDirectorNinoFormat());
    }
    
    [Fact]
    public async Task InvalidShareholderName_ValidateAsync_ReturnsError()
    {
        _model.Shareholders.Shareholder1.Shareholder1FullName = new string('X', 101);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, ShareholderValidationInfo.InvalidShareholderNameLength());
    }
    
    [Fact]
    public async Task InvalidShareholderPercentage_ValidateAsync_ReturnsError()
    {
        _model.Shareholders.Shareholder1.Shareholder1Percentage = 50.123M;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, ShareholderValidationInfo.InvalidShareholderPercentage());
    }
    
    [Fact]
    public async Task InvalidAssociatedCompanyName_ValidateAsync_ReturnsError()
    {
        _model.AssociatedCompanies.AssociatedCompany1.AssocCompany1Name = new string('X', 61);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNameLength());
    }
    
    [Fact]
    public async Task InvalidAssociatedCompanyNumber_ValidateAsync_ReturnsError()
    {
        _model.AssociatedCompanies.AssociatedCompany1.AssocCompany1Number = new string('X', 10);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNumberLength());
    }
    
    [Fact]
    public async Task InvalidAssociatedCompanyReason_ValidateAsync_ReturnsError()
    {
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1ReasonForAssociation = new string('X', 256);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AssociatedCompanyValidationInfo.InvalidAssociationReasonLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingAssociatedCompanyAddressLine1_ValidateAsync_ReturnsError(string? line1)
    {
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrLine1 = line1;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.MissingAddressLine1("Associated company"));
    }
    
    [Fact]
    public async Task InvalidAssociatedCompanyAddress_ValidateAsync_ReturnsError()
    {
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrLine1 = new string('X', 36);
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrLine2 = new string('X', 36);
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrTown = new string('X', 36);
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrCounty = new string('X', 36);
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrPostcode = new string('X', 11);
        _model.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrCountry = new string('X', 11);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Associated company"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Associated company"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressTownLength("Associated company"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountyLength("Associated company"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressPostcodeLength("Associated company"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountryLength("Associated company"));
    }
    
    [Fact]
    public async Task InvalidContinuityEmployerName_ValidateAsync_ReturnsError()
    {
        _model.Employees.EmployeesClaimingContinuity.EmployerName = new string('X', 61);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, EmploymentContinuityValidationInfo.InvalidContinuityEmployerNameLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingContinuityEmployerAddressLine1_ValidateAsync_ReturnsError(string? line1)
    {
        _model.Employees.EmployeesClaimingContinuity.EmployerAddrLine1 = line1;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.MissingAddressLine1("Employment continuity"));
    }
    
    [Fact]
    public async Task InvalidContinuityEmployerAddress_ValidateAsync_ReturnsError()
    {
        _model.Employees.EmployeesClaimingContinuity.EmployerAddrLine1 = new string('X', 36);
        _model.Employees.EmployeesClaimingContinuity.EmployerAddrLine2 = new string('X', 36);
        _model.Employees.EmployeesClaimingContinuity.EmployerAddrTown = new string('X', 36);
        _model.Employees.EmployeesClaimingContinuity.EmployerAddrCounty = new string('X', 36);
        _model.Employees.EmployeesClaimingContinuity.EmployerAddrPostcode = new string('X', 11);
        _model.Employees.EmployeesClaimingContinuity.EmployerAddrCountry = new string('X', 11);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Employment continuity"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Employment continuity"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressTownLength("Employment continuity"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountyLength("Employment continuity"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressPostcodeLength("Employment continuity"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountryLength("Employment continuity"));
    }
    
    [Fact]
    public async Task InvalidTransferToName_ValidateAsync_ReturnsError()
    {
        _model.TransferDetails.TransferTo.Name = new string('X', 61);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, TransfersValidationInfo.InvalidTransferToNameLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingTransferToAddressLine1_ValidateAsync_ReturnsError(string? line1)
    {
        _model.TransferDetails.TransferTo.TransferToAddrLine1 = line1;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.MissingAddressLine1("Transfers"));
    }
    
    [Fact]
    public async Task InvalidTransferToAddress_ValidateAsync_ReturnsError()
    {
        _model.TransferDetails.TransferTo.TransferToAddrLine1 = new string('X', 36);
        _model.TransferDetails.TransferTo.TransferToAddrLine2 = new string('X', 36);
        _model.TransferDetails.TransferTo.TransferToAddrTown = new string('X', 36);
        _model.TransferDetails.TransferTo.TransferToAddrCounty = new string('X', 36);
        _model.TransferDetails.TransferTo.TransferToAddrPostcode = new string('X', 11);
        _model.TransferDetails.TransferTo.TransferToAddrCountry = new string('X', 11);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Transfers"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Transfers"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressTownLength("Transfers"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountyLength("Transfers"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressPostcodeLength("Transfers"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountryLength("Transfers"));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingPayRecordsContactName_ValidateAsync_ReturnsError(string? name)
    {
        _model.PayRecordsContact.Name = name;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, PayRecordsContactValidationInfo.MissingPayRecordName());
    }
    
    [Fact]
    public async Task InvalidPayRecordsContactName_ValidateAsync_ReturnsError()
    {
        _model.PayRecordsContact.Name = new string('X', 61);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, PayRecordsContactValidationInfo.InvalidPayRecordNameLength());
    }
    
    [Fact]
    public async Task InvalidPayRecordsContactEmail_ValidateAsync_ReturnsError()
    {
        _model.PayRecordsContact.PayRecordsEmailAddress = new string('X', 101);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, PayRecordsContactValidationInfo.InvalidPayRecordEmailLength());
    }
    
    [Fact]
    public async Task InvalidPayRecordsContactPhone_ValidateAsync_ReturnsError()
    {
        _model.PayRecordsContact.PayRecordsPhoneNumber = new string('X', 13);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, PayRecordsContactValidationInfo.InvalidPayRecordPhoneLength());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MissingPayRecordsContactAddressLine1_ValidateAsync_ReturnsError(string? line1)
    {
        _model.PayRecordsContact.PayRecordsAddrLine1 = line1;
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.MissingAddressLine1("Pay records contact"));
    }
    
    [Fact]
    public async Task InvalidPayRecordsContactAddress_ValidateAsync_ReturnsError()
    {
        _model.PayRecordsContact.PayRecordsAddrLine1 = new string('X', 36);
        _model.PayRecordsContact.PayRecordsAddrLine2 = new string('X', 36);
        _model.PayRecordsContact.PayRecordsAddrTown = new string('X', 36);
        _model.PayRecordsContact.PayRecordsAddrCounty = new string('X', 36);
        _model.PayRecordsContact.PayRecordsAddrPostcode = new string('X', 11);
        _model.PayRecordsContact.PayRecordsAddrCountry = new string('X', 11);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Pay records contact"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Pay records contact"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressTownLength("Pay records contact"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountyLength("Pay records contact"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressPostcodeLength("Pay records contact"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountryLength("Pay records contact"));
    }
    
    [Fact]
    public async Task InvalidIPRegistrationNumber_ValidateAsync_ReturnsError()
    {
        _model.InsolvencyPractitioner.IPRegistrationNumber = new string('X', 10);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, InsolvencyPractitionerValidationInfo.InvalidIPRegistrationNumberLength());
    }
    
    [Fact]
    public async Task InvalidIPFirmName_ValidateAsync_ReturnsError()
    {
        _model.InsolvencyPractitioner.IPFirmName = new string('X', 256);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, InsolvencyPractitionerValidationInfo.InvalidIPFirmNameLength());
    }
    
    [Fact]
    public async Task InvalidIPName_ValidateAsync_ReturnsError()
    {
        _model.InsolvencyPractitioner.IPName = new string('X', 61);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, InsolvencyPractitionerValidationInfo.InvalidIPNameLength());
    }
    
    [Fact]
    public async Task InvalidIPEmail_ValidateAsync_ReturnsError()
    {
        _model.InsolvencyPractitioner.IPEmailAddress = new string('X', 101);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, InsolvencyPractitionerValidationInfo.InvalidIPEmailLength());
    }
    
    [Fact]
    public async Task InvalidIPPhone_ValidateAsync_ReturnsError()
    {
        _model.InsolvencyPractitioner.IPTelephoneNumber = new string('X', 41);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, InsolvencyPractitionerValidationInfo.InvalidIPPhoneLength());
    }
    
    [Fact]
    public async Task InvalidIPAddress_ValidateAsync_ReturnsError()
    {
        _model.InsolvencyPractitioner.IPAddressLine1 = new string('X', 36);
        _model.InsolvencyPractitioner.IPAddressLine2 = new string('X', 36);
        _model.InsolvencyPractitioner.IPAddressTown = new string('X', 36);
        _model.InsolvencyPractitioner.IPAddressCounty = new string('X', 36);
        _model.InsolvencyPractitioner.IPAddressPostcode = new string('X', 11);
        _model.InsolvencyPractitioner.IPAddressCountry = new string('X', 11);
        
        ValidatorContext context = await _validator.ValidateAsync();

        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Insolvency practitioner"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressLineLength("Insolvency practitioner"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressTownLength("Insolvency practitioner"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountyLength("Insolvency practitioner"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressPostcodeLength("Insolvency practitioner"));
        EmployerSpreadsheetHelper.AssertError(context.Errors, AddressValidationInfo.InvalidAddressCountryLength("Insolvency practitioner"));
    }
    
    [Fact]
    public async Task ValidModel_ValidateAsync_ReturnsNoErrors()
    {
        ValidatorContext context = await _validator.ValidateAsync();

        Assert.Empty(context.Errors);
    }
}