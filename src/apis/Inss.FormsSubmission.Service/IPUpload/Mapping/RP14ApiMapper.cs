using System.Text.Json;
using Inss.Common.IPUpload.Employer.Api;
using Inss.FormsSubmission.Service.IPUpload.Employer;
using Inss.FormsSubmission.Service.IPUpload.Exceptions;

namespace Inss.FormsSubmission.Service.IPUpload.Mapping;

public sealed class RP14ApiMapper : IMapper
{
    private readonly RP14 _model;
    
    public RP14ApiMapper(RP14 model)
    {
        _model = model;
    }
    
    public JsonMessage[] Map()
    {
        // NOTE: Logic lifted from https://github.com/InsolvencyService/RedundancyUploadService/blob/develop/Insolvency.RedundancyUploadService.BL/Mappers/RP14ApiMessageMapper.cs
        
        CompanyInformation companyInformation = new()
        {
            CorrelationId = Guid.NewGuid(),
            CaseReference = _model.Header?.CaseReference ?? null!,
            Company = MapCompany(_model),
            Directors = MapDirectors(_model.Directors.Director),
            DirectorsClaimingRedundancy = _model.Directors.ClaimingRPAsEmployeeSpecified 
                ? MapYesNo(_model.Directors.ClaimingRPAsEmployee) : null,
            Shareholders = MapShareholders(_model.Shareholders),
            LegallyAssociatedCompanies = _model.AssociatedCompanies?.LegallyAssociatedCompaniesSpecified == true 
                ? MapYesNo(_model.AssociatedCompanies.LegallyAssociatedCompanies) : null,
            AssociatedCompanies = MapAssociatedCompanies(_model.AssociatedCompanies?.AssociatedCompany ?? []),
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
            Address = MapAddress(model.Address),
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
            PreviousEmployer = MapPreviousEmployer(employees)
        };
    }

    private static PayRecordsContact MapPayRecordsContact(RP14PayRecordsContact recordsContact)
    {
        return new PayRecordsContact
        {
            Name = recordsContact.Name,
            PayRecordsAddress = MapAddress(recordsContact.Address),
            TelephoneNumber = recordsContact.PhoneNumber,
            EmailAddress = recordsContact.EmailAddress
        };
    }

    private static InsolvencyPractitioner MapInsolvencyPractitioner(RP14InsolvencyPractitioner insolvencyPractitioner)
    {
        return new InsolvencyPractitioner
        {
            IpRegistrationNumber = insolvencyPractitioner.RegistrationNumber,
            IpFirmName = insolvencyPractitioner.FirmName,
            IpName = insolvencyPractitioner.Name,
            DeclarationAgreement = insolvencyPractitioner.DeclarationAgreement,
            Address = MapAddress(insolvencyPractitioner.Address),
            TelephoneNumber = insolvencyPractitioner.TelephoneNumber,
            EmailAddress = insolvencyPractitioner.EmailAddress
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
                Address = transferDetails.TransferTo is not null ? MapAddress(transferDetails.TransferTo.Address) : null!,
                TransferDate = transferDetails.TransferDateSpecified ? transferDetails.TransferDate : null,
                DateNegotiationBegan = transferDetails.NegotiationDateSpecified ? transferDetails.NegotiationDate : null
            }
        };
    }
    
    private static List<Director> MapDirectors(RP14DirectorsDirector[] directors)
    {
        return directors
            .Where(d => d.Name is not null && !string.IsNullOrEmpty(d.Name.Surname))
            .Select(d => new Director
            {
                Initials = d.Name.Initials,
                Surname = d.Name.Surname,
                NationalInsuranceNumber = d.NINO.Replace(" ", string.Empty).ToUpper()
            })
            .ToList();
    }

    private static List<Shareholder> MapShareholders(RP14Shareholder[] shareholders)
    {
        return shareholders
            .Select(s => new Shareholder
            {
                Percentage = s.PercentageSpecified ? s.Percentage : null,
                Fullname = s.Name.FullName,
                NumberOfSharesHeld = int.TryParse(s.NoOfSharesHeld, out int noShares) ? noShares : null
            })
            .ToList();
    }

    private static List<AssociatedCompany> MapAssociatedCompanies(RP14AssociatedCompaniesAssociatedCompany[] associatedCompanies)
    {
        return associatedCompanies
            .Select(ac => new AssociatedCompany
            {
                Name = ac.CompanyName,
                Number = ac.CompanyNumber,
                Address = MapAddress(ac.Address),
                OfferMade = ac.OfferToEmployEmployees is not null && (MapYesNo(ac.OfferToEmployEmployees.OfferMade) ?? false)
            })
            .ToList();
    }

    private static PreviousEmployer MapPreviousEmployer(RP14Employees employees)
    {
        RP14EmployeesEmployeesClaimingContinuity? employeesClaimingContinuity = employees.EmployeesClaimingContinuity;
        
        return new PreviousEmployer
        {
            EmployerName = employeesClaimingContinuity?.EmployerName ?? null!,
            Address = employeesClaimingContinuity is not null ? MapAddress(employeesClaimingContinuity.Address) : null!,
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

    private static Address MapAddress(AddressType address)
    {
        return new Address
        {
            Line1 = address.Line.Length > 0 ? address.Line[0] : null!,
            Line2 = address.Line.Length > 1 ? address.Line[1] : null!,
            Line3 = address.Line.Length > 2 ? address.Line[2] : null!,
            Town = address.Town,
            County = address.County,
            Postcode = address.Postcode,
            Country = address.Country
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