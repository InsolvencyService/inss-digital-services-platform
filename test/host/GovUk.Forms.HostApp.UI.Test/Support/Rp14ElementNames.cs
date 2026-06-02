namespace GovUk.Forms.HostApp.UI.Test.Support;

public static class Rp14ElementNames
{
    public const string CaseReference = "CaseReference";

    public const string NameOfBusiness = "NameOfBusiness";
    public const string CompanyNumber = "CompanyNumber";
    public const string IncorporationDate = "IncorporationDate";
    public const string NatureOfBusiness = "NatureOfBusiness";

    public const string PayeDistrict = "District";
    public const string PayeReference = "Reference";

    public const string NoOfEmployees = "NoOfEmployees";
    public const string EmployerName = "EmployerName";

    public const string InsolvencyDate = "InsolvencyDate";
    public const string InsolvencyType = "InsolvencyType";

    public const string TransferType = "TransferType";
    public const string TransferToName = "Name";
    public const string TransferDate = "TransferDate";
    public const string NegotiationDate = "NegotiationDate";

    public const string IpRegistrationNumber = "IPRegistrationNumber";
    public const string IpFirmName = "IPFirmName";
    public const string IpName = "IPName";
    public const string IpEmailAddress = "IPEmailAddress";
    public const string IpTelephoneNumber = "IPTelephoneNumber";
    public const string StandardIndustrialClassification = "SICCode";

    public const string PayRecordsContact = "PayRecordsContact";
    public const string PayRecordsContactName = "Name";
    public const string PayRecordsPhoneNumber = "PayRecordsPhoneNumber";
    public const string PayRecordsEmailAddress = "PayRecordsEmailAddress";

    public static string DirectorSurname(int n) => $"Director{n}Surname";
    public static string DirectorInitials(int n) => $"Director{n}Initials";
    public static string DirectorNino(int n) => $"Director{n}NINO";

    public static string ShareholderFullName(int n) => $"Shareholder{n}FullName";
    public static string ShareholderNoOfShares(int n) => $"Shareholder{n}NoOfSharesHeld";
    public static string ShareholderPercentage(int n) => $"Shareholder{n}Percentage";

    public static string AssociatedCompanyName(int n) => $"AssocCompany{n}Name";
    public static string AssociatedCompanyReason(int n) => $"AssocCompany{n}Reason";
    public static string AssociatedCompanyNumber(int n) => $"AssocCompany{n}Number";

    public const string CompanyAddrLine1 = "CompanyAddrLine1";
    public const string CompanyAddrTown = "CompanyAddrTown";
    public const string CompanyAddrCounty = "CompanyAddrCounty";
    public const string CompanyAddrPostcode = "CompanyAddrPostcode";
    public const string CompanyAddrCountry = "CompanyAddrCountry";

    public static string CompanyAddressLine(int n) => $"CompanyAddrLine{n}";
}
