using System.Globalization;
using System.Text.Json;
using Inss.Common.IPUpload.Employer.Spreadsheet;
using Inss.FormsSubmission.Service.IPUpload.Employer;
using Inss.FormsSubmission.Service.IPUpload.Exceptions;

namespace Inss.FormsSubmission.Service.IPUpload.Mapping;

public sealed class RP14SpreadsheetMapper : IMapper
{
    private readonly RP14 _model;
    
    public RP14SpreadsheetMapper(RP14 model)
    {
        _model = model;
    }
    
    public JsonMessage[] Map()
    {
        // NOTE: Logic lifted from https://github.com/InsolvencyService/RedundancyUploadService/blob/develop/Insolvency.RedundancyUploadService.BL/Mappers/RP14SpreadSheetMessageMapper.cs
        
        CompanyInformation companyInformation = new()
        {
            CorrelationId = Guid.NewGuid(),
            CaseReference = _model.Header?.CaseReference ?? null!,
            Company = MapCompany(_model),
            Directors = MapDirectors(_model.Directors),
            DirectorsClaimingRedundancy = _model.Directors.ClaimingRPAsEmployeeSpecified 
                ? MapYesNo(_model.Directors.ClaimingRPAsEmployee) : null,
            Shareholders = MapShareholders(_model.Shareholders),
            LegallyAssociatedCompanies = _model.AssociatedCompanies?.LegallyAssociatedCompaniesSpecified == true 
                ? MapYesNo(_model.AssociatedCompanies.LegallyAssociatedCompanies) : null,
            AssociatedCompanies = MapAssociatedCompanies(_model.AssociatedCompanies),
            Insolvency = MapInsolvency(_model.InsolvencyDetails),
            Transfer = MapTransferDetails(_model.TransferDetails),
            Paye = MapPaye(_model.PAYE),
            Employees = MapEmployees(_model.Employees),
            PayRecordsContact = MapPayRecordsContact(_model.PayRecordsContact),
            InsolvencyPractitioner = MapInsolvencyPractitioner(_model.InsolvencyPractitioner)
        };

        return
        [
            new JsonMessage
            {
                CorrelationId = companyInformation.CorrelationId.ToString(),
                Json = JsonSerializer.Serialize(companyInformation),
                Entity = "inss_inboundemployermessages",
                MessageName = "Inbound Employer Message"
            }
        ];
    }

    private static Company MapCompany(RP14 model)
    {
        return new Company
        {
            Name = model.NameOfBusiness,
            Number = model.CompanyNumber,
            IncorporationDate = model.IncorporationDateSpecified ? model.IncorporationDate : null,
            Address = new Address
            {
                Line1 = model.CompanyAddrLine1,
                Line2 = model.CompanyAddrLine2,
                Line3 = model.CompanyAddrLine3,
                Town = model.CompanyAddrTown,
                County = model.CompanyAddrCounty,
                Postcode = model.CompanyAddrPostcode,
                Country = model.CompanyAddrCountry
            },
            NatureOfBusiness = model.NatureOfBusiness
        };
    }

    private static Insolvency MapInsolvency(RP14InsolvencyDetails insolvencyDetails)
    {
        return new Insolvency
        {
            InsolvencyDate = insolvencyDetails.InsolvencyDateSpecified ? insolvencyDetails.InsolvencyDate : null,
            InsolvencyType = insolvencyDetails.InsolvencyType
        };
    }

    private static Paye MapPaye(PayeType paye)
    {
        return new Paye
        {
            District = paye.District,
            Reference = paye.Reference
        };
    }

    private static Employees MapEmployees(RP14Employees employees)
    {
        return new Employees
        {
            NumberOfEmployees = int.TryParse(employees.NoOfEmployees, out int noEmployees) ? noEmployees : null,
            Continuity = employees.EmployeesClaimingContinuity is not null &&
                         employees.CarryOverHolidayEntitlementSpecified
                ? MapYesNo(employees.EmployeesClaimingContinuity.ClaimingContinuity) : null,
            PreviousEmployer = employees.EmployeesClaimingContinuity is not null ? MapPreviousEmployer(employees) : null!
        };
    }

    private static PayRecordsContact MapPayRecordsContact(RP14PayRecordsContact recordsContact)
    {
        return new PayRecordsContact
        {
            Name = recordsContact.Name,
            PayRecordsAddress = new Address
            {
                Line1 = recordsContact.PayRecordsAddrLine1,
                Line2 = recordsContact.PayRecordsAddrLine2,
                Line3 = recordsContact.PayRecordsAddrLine3,
                Town = recordsContact.PayRecordsAddrTown,
                County = recordsContact.PayRecordsAddrCounty,
                Postcode = recordsContact.PayRecordsAddrPostcode,
                Country = recordsContact.PayRecordsAddrCountry,
            },
            TelephoneNumber = recordsContact.PayRecordsPhoneNumber,
            EmailAddress = recordsContact.PayRecordsEmailAddress
        };
    }

    private static InsolvencyPractitioner MapInsolvencyPractitioner(RP14InsolvencyPractitioner insolvencyPractitioner)
    {
        return new InsolvencyPractitioner
        {
            IpRegistrationNumber = insolvencyPractitioner.IPRegistrationNumber,
            IpFirmName = insolvencyPractitioner.IPFirmName,
            IpName = insolvencyPractitioner.IPName,
            DeclarationAgreement = insolvencyPractitioner.DeclarationAgreement,
            Address = new Address
            {
                Line1 = insolvencyPractitioner.IPAddressLine1,
                Line2 = insolvencyPractitioner.IPAddressLine2,
                Line3 = insolvencyPractitioner.IPAddressLine3,
                Town = insolvencyPractitioner.IPAddressTown,
                County = insolvencyPractitioner.IPAddressCounty,
                Postcode = insolvencyPractitioner.IPAddressPostcode,
                Country = insolvencyPractitioner.IPAddressCountry
            },
            TelephoneNumber = insolvencyPractitioner.IPTelephoneNumber,
            EmailAddress = insolvencyPractitioner.IPEmailAddress
        };
    }
    
    private static Transfer MapTransferDetails(RP14TransferDetails transferDetails)
    {
        return new Transfer
        {
            Type = transferDetails.TransferTypeSpecified ? MapTransferType(transferDetails.TransferType) : null!,
            To = new TransferTo
            {
                Name = transferDetails.TransferTo?.Name ?? null!,
                Address = transferDetails.TransferTo is not null
                    ? new Address
                    {
                        Line1 = transferDetails.TransferTo.TransferToAddrLine1,
                        Line2 = transferDetails.TransferTo.TransferToAddrLine2,
                        Line3 = transferDetails.TransferTo.TransferToAddrLine3,
                        County = transferDetails.TransferTo.TransferToAddrCounty,
                        Postcode = transferDetails.TransferTo.TransferToAddrPostcode,
                        Country = transferDetails.TransferTo.TransferToAddrCountry
                    }
                    : null!,
                TransferDate = transferDetails.TransferDateSpecified ? transferDetails.TransferDate : null,
                DateNegotiationBegan = transferDetails.NegotiationDateSpecified ? transferDetails.NegotiationDate : null
            }
        };
    }
    
    private static List<Director> MapDirectors(RP14Directors? directors)
    {
        if (directors is null)
        {
            return [];
        }

        List<Director> directorList = [];
        
        if (directors.Director1 is not null && !string.IsNullOrEmpty(directors.Director1.Director1Surname))
        {
            directorList.Add(new Director
            {
                Initials = directors.Director1.Director1Initials,
                Surname = directors.Director1.Director1Surname,
                NationalInsuranceNumber = directors.Director1.Director1NINO?.ToUpper().Replace(" ", string.Empty) ?? null!
            });
        }
        
        if (directors.Director2 is not null && !string.IsNullOrEmpty(directors.Director2.Director2Surname))
        {
            directorList.Add(new Director
            {
                Initials = directors.Director2.Director2Initials,
                Surname = directors.Director2.Director2Surname,
                NationalInsuranceNumber = directors.Director2.Director2NINO?.ToUpper().Replace(" ", string.Empty) ?? null!
            });
        }
        
        if (directors.Director3 is not null && !string.IsNullOrEmpty(directors.Director3.Director3Surname))
        {
            directorList.Add(new Director
            {
                Initials = directors.Director3.Director3Initials,
                Surname = directors.Director3.Director3Surname,
                NationalInsuranceNumber = directors.Director3.Director3NINO?.ToUpper().Replace(" ", string.Empty) ?? null!
            });
        }
        
        if (directors.Director4 is not null && !string.IsNullOrEmpty(directors.Director4.Director4Surname))
        {
            directorList.Add(new Director
            {
                Initials = directors.Director4.Director4Initials,
                Surname = directors.Director4.Director4Surname,
                NationalInsuranceNumber = directors.Director4.Director4NINO?.ToUpper().Replace(" ", string.Empty) ?? null!
            });
        }
        
        if (directors.Director5 is not null && !string.IsNullOrEmpty(directors.Director5.Director5Surname))
        {
            directorList.Add(new Director
            {
                Initials = directors.Director5.Director5Initials,
                Surname = directors.Director5.Director5Surname,
                NationalInsuranceNumber = directors.Director5.Director5NINO?.ToUpper().Replace(" ", string.Empty) ?? null!
            });
        }
        
        if (directors.Director6 is not null && !string.IsNullOrEmpty(directors.Director6.Director6Surname))
        {
            directorList.Add(new Director
            {
                Initials = directors.Director6.Director6Initials,
                Surname = directors.Director6.Director6Surname,
                NationalInsuranceNumber = directors.Director6.Director6NINO?.ToUpper().Replace(" ", string.Empty) ?? null!
            });
        }

        return directorList;
    }

    private static List<Shareholder> MapShareholders(RP14Shareholders? shareholders)
    {
        if (shareholders is null)
        {
            return [];
        }
        
        List<Shareholder> shareholderList = [];

        if (shareholders.Shareholder1 is { Shareholder1FullName: not null, Shareholder1PercentageSpecified: true } &&
            int.TryParse(shareholders.Shareholder1.Shareholder1NoOfSharesHeld, out int _))
        {
            shareholderList.Add(new Shareholder
            {
                Percentage = shareholders.Shareholder1.Shareholder1Percentage,
                Fullname = shareholders.Shareholder1.Shareholder1FullName,
                NumberOfSharesHeld = int.Parse(
                    shareholders.Shareholder1.Shareholder1NoOfSharesHeld, NumberStyles.Integer, CultureInfo.CurrentCulture)
            });
        }
        
        if (shareholders.Shareholder2 is { Shareholder2FullName: not null, Shareholder2PercentageSpecified: true } &&
            int.TryParse(shareholders.Shareholder2.Shareholder2NoOfSharesHeld, out int _))
        {
            shareholderList.Add(new Shareholder
            {
                Percentage = shareholders.Shareholder2.Shareholder2Percentage,
                Fullname = shareholders.Shareholder2.Shareholder2FullName,
                NumberOfSharesHeld = int.Parse(
                    shareholders.Shareholder2.Shareholder2NoOfSharesHeld, NumberStyles.Integer, CultureInfo.CurrentCulture)
            });
        }
        
        if (shareholders.Shareholder3 is { Shareholder3FullName: not null, Shareholder3PercentageSpecified: true } &&
            int.TryParse(shareholders.Shareholder3.Shareholder3NoOfSharesHeld, out int _))
        {
            shareholderList.Add(new Shareholder
            {
                Percentage = shareholders.Shareholder3.Shareholder3Percentage,
                Fullname = shareholders.Shareholder3.Shareholder3FullName,
                NumberOfSharesHeld = int.Parse(
                    shareholders.Shareholder3.Shareholder3NoOfSharesHeld, NumberStyles.Integer, CultureInfo.CurrentCulture)
            });
        }
        
        if (shareholders.Shareholder4 is { Shareholder4FullName: not null, Shareholder4PercentageSpecified: true } &&
            int.TryParse(shareholders.Shareholder4.Shareholder4NoOfSharesHeld, out int _))
        {
            shareholderList.Add(new Shareholder
            {
                Percentage = shareholders.Shareholder4.Shareholder4Percentage,
                Fullname = shareholders.Shareholder4.Shareholder4FullName,
                NumberOfSharesHeld = int.Parse(
                    shareholders.Shareholder4.Shareholder4NoOfSharesHeld, NumberStyles.Integer, CultureInfo.CurrentCulture)
            });
        }
        
        if (shareholders.Shareholder5 is { Shareholder5FullName: not null, Shareholder5PercentageSpecified: true } &&
            int.TryParse(shareholders.Shareholder5.Shareholder5NoOfSharesHeld, out int _))
        {
            shareholderList.Add(new Shareholder
            {
                Percentage = shareholders.Shareholder5.Shareholder5Percentage,
                Fullname = shareholders.Shareholder5.Shareholder5FullName,
                NumberOfSharesHeld = int.Parse(
                    shareholders.Shareholder5.Shareholder5NoOfSharesHeld, NumberStyles.Integer, CultureInfo.CurrentCulture)
            });
        }
        
        if (shareholders.Shareholder6 is { Shareholder6FullName: not null, Shareholder6PercentageSpecified: true } &&
            int.TryParse(shareholders.Shareholder6.Shareholder6NoOfSharesHeld, out int _))
        {
            shareholderList.Add(new Shareholder
            {
                Percentage = shareholders.Shareholder6.Shareholder6Percentage,
                Fullname = shareholders.Shareholder6.Shareholder6FullName,
                NumberOfSharesHeld = int.Parse(
                    shareholders.Shareholder6.Shareholder6NoOfSharesHeld, NumberStyles.Integer, CultureInfo.CurrentCulture)
            });
        }
        
        return shareholderList;
    }

    private static List<AssociatedCompany> MapAssociatedCompanies(RP14AssociatedCompanies? associatedCompanies)
    {
        if (associatedCompanies is null)
        {
            return [];
        }

        List<AssociatedCompany> associatedCompanyList = [];
        
        if (associatedCompanies.AssociatedCompany1 is not null)
        {
            associatedCompanyList.Add(new AssociatedCompany
            {
                Name = associatedCompanies.AssociatedCompany1.AssocCompany1Name,
                Number = associatedCompanies.AssociatedCompany1.AssocCompany1Number,
                Address = new Address
                {
                    Line1 = associatedCompanies.AssociatedCompany1.AssocComp1AddrLine1,
                    Line2 = associatedCompanies.AssociatedCompany1.AssocComp1AddrLine2,
                    Line3 = associatedCompanies.AssociatedCompany1.AssocComp1AddrLine3,
                    Town = associatedCompanies.AssociatedCompany1.AssocComp1AddrTown,
                    County = associatedCompanies.AssociatedCompany1.AssocComp1AddrCounty,
                    Postcode = associatedCompanies.AssociatedCompany1.AssocComp1AddrPostcode,
                    Country = associatedCompanies.AssociatedCompany1.AssocComp1AddrCountry
                },
                OfferMade = associatedCompanies.AssociatedCompany1.AssocComp1OfferToEmployEmployees is not null && 
                            (MapYesNo(associatedCompanies.AssociatedCompany1.AssocComp1OfferToEmployEmployees.AssocComp1OfferMade) ?? false)
            });
        }

        if (associatedCompanies.AssociatedCompany2 is not null)
        {
            associatedCompanyList.Add(new AssociatedCompany
            {
                Name = associatedCompanies.AssociatedCompany2.AssocCompany2Name,
                Number = associatedCompanies.AssociatedCompany2.AssocCompany2Number,
                Address = new Address
                {
                    Line1 = associatedCompanies.AssociatedCompany2.AssocComp2AddrLine1,
                    Line2 = associatedCompanies.AssociatedCompany2.AssocComp2AddrLine2,
                    Line3 = associatedCompanies.AssociatedCompany2.AssocComp2AddrLine3,
                    Town = associatedCompanies.AssociatedCompany2.AssocComp2AddrTown,
                    County = associatedCompanies.AssociatedCompany2.AssocComp2AddrCounty,
                    Postcode = associatedCompanies.AssociatedCompany2.AssocComp2AddrPostcode,
                    Country = associatedCompanies.AssociatedCompany2.AssocComp2AddrCountry
                },
                OfferMade = associatedCompanies.AssociatedCompany2.AssocComp2OfferToEmployEmployees is not null && 
                            (MapYesNo(associatedCompanies.AssociatedCompany2.AssocComp2OfferToEmployEmployees.AssocComp2OfferMade) ?? false)
            });
        }
        
        return associatedCompanyList;
    }

    private static PreviousEmployer MapPreviousEmployer(RP14Employees employees)
    {
        RP14EmployeesEmployeesClaimingContinuity? employeesClaimingContinuity = employees.EmployeesClaimingContinuity;
        
        return new PreviousEmployer
        {
            EmployerName = employeesClaimingContinuity?.EmployerName ?? null!,
            Address = employeesClaimingContinuity is not null
                ? new Address
                {
                    Line1 = employeesClaimingContinuity.EmployerAddrLine1,
                    Line2 = employeesClaimingContinuity.EmployerAddrLine2,
                    Line3 = employeesClaimingContinuity.EmployerAddrLine3,
                    Town = employeesClaimingContinuity.EmployerAddrTown,
                    County = employeesClaimingContinuity.EmployerAddrCounty,
                    Postcode = employeesClaimingContinuity.EmployerAddrPostcode,
                    Country = employeesClaimingContinuity.EmployerAddrCountry
                }
                : null!,
            DateTradingStarted = employeesClaimingContinuity is not null && 
                                 employeesClaimingContinuity.TradingDateSpecified
                ? employeesClaimingContinuity.TradingDate : null,
            ShouldClaimsBeAccepted = employeesClaimingContinuity is not null && 
                                     employeesClaimingContinuity.ShouldClaimsBeAcceptedSpecified 
                ? MapYesNo(employeesClaimingContinuity.ShouldClaimsBeAccepted) : null,
            Strikes = employees.StrikesSpecified ? MapYesNo(employees.Strikes) : null,
            EntitledToCarryOverHoliday = employees.CarryOverHolidayEntitlementSpecified 
                ? MapYesNo(employees.CarryOverHolidayEntitlement) : null
        };
    }
    
    private static bool? MapYesNo(YesNoType? yesNo)
    {
        if (!yesNo.HasValue)
        {
            return null;
        }

        return yesNo.Value switch
        {
            YesNoType.yes => true,
            YesNoType.no => false,
            _ => throw new IPUploadMappingException($"Cannot convert {yesNo} to bool.")
        };
    }
    
    private static string MapTransferType(RP14TransferDetailsTransferType? transferType)
    {
        if (!transferType.HasValue)
        {
            return null!;
        }

        return transferType.Value switch
        {
            RP14TransferDetailsTransferType.assetonly => "asset only",
            RP14TransferDetailsTransferType.wholebusiness => "whole business",
            RP14TransferDetailsTransferType.partofbusiness => "part of business",
            RP14TransferDetailsTransferType.other => "other",
            RP14TransferDetailsTransferType.none => "none",
            _ => throw new IPUploadMappingException($"Cannot convert {transferType} to string.")
        };
    }
}