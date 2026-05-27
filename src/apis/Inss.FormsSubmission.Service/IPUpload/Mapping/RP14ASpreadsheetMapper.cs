using System.Text.Json;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.FormsSubmission.Service.IPUpload.Employee;
using Inss.FormsSubmission.Service.IPUpload.Exceptions;

namespace Inss.FormsSubmission.Service.IPUpload.Mapping;

public sealed class RP14ASpreadsheetMapper : IMapper
{
    private readonly RP14A _model;
    
    public RP14ASpreadsheetMapper(RP14A model)
    {
        _model = model;
    }
    
    public JsonMessage[] Map()
    {
        // NOTE: Logic lifted from https://github.com/InsolvencyService/RedundancyUploadService/blob/develop/Insolvency.RedundancyUploadService.BL/Mappers/RP14aSpreadSheetMessageMapper.cs
        
        EmployeeInformation[] employeeInformationList = _model.Employee.Select(e => new EmployeeInformation
        {
            CorrelationId = Guid.NewGuid(),
            CaseReference = e.Header.CaseReference,
            EmployerName = e.EmployerName,
            Employee = new Employee.Employee
            {
                Title = e.EmployeeName.Title,
                FirstNames = e.EmployeeName.Forenames,
                LastName = e.EmployeeName.Surname,
                NationalInsuranceNumber = e.NINO.ToUpper(),
                DateOfBirth = e.DateOfBirthSpecified ? e.DateOfBirth : null,
                StartDate = e.StartDateSpecified ? e.StartDate : null,
                EndDate = e.EndDateSpecified ? e.EndDate : null,
                DateNoticeGiven = e.DateNoticeGivenSpecified ? e.DateNoticeGiven : null,
                Pay = MapPay(e.PayDetails, e.Holiday),
                IsDirector = e.IsDirectorSpecified ? e.IsDirector.ToString() : null!,
                AverageHoursWorked = e.AverageHoursWorkedSpecified ? e.AverageHoursWorked : null,
                MoneyOwedToEmployer = e.MoneyOwedToEmployerSpecified ? e.MoneyOwedToEmployer : null,
                EntitledToRedundancyPay = e.EntitledToRedundancyPaySpecified ? e.EntitledToRedundancyPay.ToString() : null!,
                EntitledToNoticePay = e.EntitledToNoticePaySpecified ? e.EntitledToNoticePay.ToString() : null!
            }
        }).ToArray();

        return employeeInformationList
            .Select(e => new JsonMessage
            {
                CorrelationId = e.CorrelationId.ToString(), 
                Json = JsonSerializer.Serialize(e),
                Entity = "inss_inboundemployeeinformationmessages",
                MessageName = "Inbound Employee Information Message"
            })
            .ToArray();
    }

    private static Pay MapPay(RP14AEmployeePayDetails payDetails, RP14AEmployeeHoliday holiday)
    {
        return new Pay
        {
            PayPerWeek = payDetails.BasicPayPerWeekSpecified ? payDetails.BasicPayPerWeek : null,
            WeeklyPayDay = payDetails.WeeklyPayDaySpecified ? payDetails.WeeklyPayDay.ToString() : null!,
            ComponentPayPerWeek = MapComponentPayPerWeekList(payDetails),
            ArrearsOfPay = MapArrearsOfPayList(payDetails.ArrearsOfPay),
            Holiday = MapHoliday(holiday)
        };
    }
    private static Holiday MapHoliday(RP14AEmployeeHoliday holiday)
    {
        return new Holiday
        {
            YearStart = holiday.HolidayYearStartSpecified ? holiday.HolidayYearStart : null,
            DaysOwed = holiday.NoDaysHolidayOwedSpecified ? holiday.NoDaysHolidayOwed : null,
            TakenAndNotPaid = MapTakenAndNotPaidList(holiday),
            HolidayContractedEntitlementDays =
                holiday.HolidayContractedEntitlementDaysSpecified ? holiday.HolidayContractedEntitlementDays : null,
            HolidayDaysCarriedForward = holiday.HolidayDaysCarriedForwardSpecified ? holiday.HolidayDaysCarriedForward : null,
            HolidayDaysTaken = holiday.HolidayDaysTakenSpecified ? holiday.HolidayDaysTaken : null
        };
    }
    private static List<ComponentPayPerWeek> MapComponentPayPerWeekList(RP14AEmployeePayDetails payDetails)
    {
        List < ComponentPayPerWeek > componentPayPerWeekList = [];

        if (payDetails.ComponentPayPerWeek1 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek1.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek1.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek1.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek1.ComponentRateStatus)!
            });
        }
        
        if (payDetails.ComponentPayPerWeek2 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek2.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek2.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek2.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek2.ComponentRateStatus)!
            });
        }
        
        if (payDetails.ComponentPayPerWeek3 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek3.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek3.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek3.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek3.ComponentRateStatus)!
            });
        }
        
        if (payDetails.ComponentPayPerWeek4 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek4.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek4.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek4.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek4.ComponentRateStatus)!
            });
        }
        
        if (payDetails.ComponentPayPerWeek5 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek5.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek5.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek5.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek5.ComponentRateStatus)!
            });
        }
        
        if (payDetails.ComponentPayPerWeek6 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek6.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek6.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek6.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek6.ComponentRateStatus)!
            });
        }
        
        if (payDetails.ComponentPayPerWeek7 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek7.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek7.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek7.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek7.ComponentRateStatus)!
            });
        }
        
        if (payDetails.ComponentPayPerWeek8 is not null)
        {
            componentPayPerWeekList.Add(new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(payDetails.ComponentPayPerWeek8.ComponentType)!,
                ComponentRate = payDetails.ComponentPayPerWeek8.ComponentRateSpecified 
                    ? payDetails.ComponentPayPerWeek8.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(payDetails.ComponentPayPerWeek8.ComponentRateStatus)!
            });
        }
        
        return componentPayPerWeekList;
    }
    
    private static List<ArrearsOfPay> MapArrearsOfPayList(RP14AEmployeePayDetailsArrearsOfPay arrearsOfPay)
    {
        List<ArrearsOfPay> arrearsOfPayList = [];

        if (arrearsOfPay.ArrearsOfPayPeriod1 is not null)
        {
            arrearsOfPayList.Add(new ArrearsOfPay
            {
                StartDate = arrearsOfPay.ArrearsOfPayPeriod1.AOP1StartDate,
                EndDate = arrearsOfPay.ArrearsOfPayPeriod1.AOP1EndDate,
                AmountOwed = arrearsOfPay.ArrearsOfPayPeriod1.AOPOwed1Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod1.AOPOwed1 : null,
                Paytype = arrearsOfPay.ArrearsOfPayPeriod1.AOPPayType1Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod1.AOPPayType1.ToString() : null!
            });
        }

        if (arrearsOfPay.ArrearsOfPayPeriod2 is not null)
        {
            arrearsOfPayList.Add(new ArrearsOfPay
            {
                StartDate = arrearsOfPay.ArrearsOfPayPeriod2.AOP2StartDate,
                EndDate = arrearsOfPay.ArrearsOfPayPeriod2.AOP2EndDate,
                AmountOwed = arrearsOfPay.ArrearsOfPayPeriod2.AOPOwed2Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod2.AOPOwed2 : null,
                Paytype = arrearsOfPay.ArrearsOfPayPeriod2.AOPPayType2Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod2.AOPPayType2.ToString() : null!
            });
        }

        if (arrearsOfPay.ArrearsOfPayPeriod3 is not null)
        {
            arrearsOfPayList.Add(new ArrearsOfPay
            {
                StartDate = arrearsOfPay.ArrearsOfPayPeriod3.AOP3StartDate,
                EndDate = arrearsOfPay.ArrearsOfPayPeriod3.AOP3EndDate,
                AmountOwed = arrearsOfPay.ArrearsOfPayPeriod3.AOPOwed3Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod3.AOPOwed3 : null,
                Paytype = arrearsOfPay.ArrearsOfPayPeriod3.AOPPayType3Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod3.AOPPayType3.ToString() : null!
            });
        }

        if (arrearsOfPay.ArrearsOfPayPeriod4 is not null)
        {
            arrearsOfPayList.Add(new ArrearsOfPay
            {
                StartDate = arrearsOfPay.ArrearsOfPayPeriod4.AOP4StartDate,
                EndDate = arrearsOfPay.ArrearsOfPayPeriod4.AOP4EndDate,
                AmountOwed = arrearsOfPay.ArrearsOfPayPeriod4.AOPOwed4Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod4.AOPOwed4 : null,
                Paytype = arrearsOfPay.ArrearsOfPayPeriod4.AOPPayType4Specified 
                    ? arrearsOfPay.ArrearsOfPayPeriod4.AOPPayType4.ToString() : null!
            });
        }
        
        return arrearsOfPayList;
    }

    private static List<TakenAndNotPaid> MapTakenAndNotPaidList(RP14AEmployeeHoliday holiday)
    {
        List<TakenAndNotPaid> takenAndNotPaidList = [];

        if (holiday.HolidayNotPaid?.Holiday1 is not null)
        {
            takenAndNotPaidList.Add(new TakenAndNotPaid
            {
                StartDate = holiday.HolidayNotPaid.Holiday1.Holiday1StartDate,
                EndDate = holiday.HolidayNotPaid.Holiday1.Holiday1EndDate
            });
        }
        
        if (holiday.HolidayNotPaid?.Holiday2 is not null)
        {
            takenAndNotPaidList.Add(new TakenAndNotPaid
            {
                StartDate = holiday.HolidayNotPaid.Holiday2.Holiday2StartDate,
                EndDate = holiday.HolidayNotPaid.Holiday2.Holiday2EndDate
            });
        }
        
        if (holiday.HolidayNotPaid?.Holiday3 is not null)
        {
            takenAndNotPaidList.Add(new TakenAndNotPaid
            {
                StartDate = holiday.HolidayNotPaid.Holiday3.Holiday3StartDate,
                EndDate = holiday.HolidayNotPaid.Holiday3.Holiday3EndDate
            });
        }
        
        return takenAndNotPaidList;
    }
    
    private static string? MapTransferType(ComponentType? componentType)
    {
        if (!componentType.HasValue)
        {
            return null;
        }

        return componentType.Value switch
        {
            ComponentType.ArrearsofPay => "Arrears of Pay",
            ComponentType.BasicAward => "Basic Award",
            ComponentType.CompensatoryNoticePay => "Compensatory Notice Pay",
            ComponentType.HolidayPayAccrued => "Holiday Pay Accrued",
            ComponentType.HolidayPayTakenNotPaid => "Holiday Pay Taken Not Paid",
            ComponentType.NoticeWorkedNotPaid => "Notice Worked Not Paid",
            ComponentType.ProtectiveAward => "Protective Award",
            ComponentType.RedundancyPay => "Redundancy Pay",
            ComponentType.RefundofNotionalTax => "Refund of Notional Tax",
            _ => throw new IPUploadMappingException($"Cannot convert {componentType} to string.")
        };
    }
    
    private static string? MapTransferStatus(ComponentRateStatusType? componentRateStatusType)
    {
        if (!componentRateStatusType.HasValue)
        {
            return null;
        }

        return componentRateStatusType.Value switch
        {
            ComponentRateStatusType.Datanotyetavailable => "Data not yet available",
            ComponentRateStatusType.Fixedrateofpay => "Fixed rate of pay",
            ComponentRateStatusType.Noholidaypaypayable => "No holiday pay payable",
            ComponentRateStatusType.Noratedefinedinlaw => "No rate defined in law",
            ComponentRateStatusType.Datatoberequestedfromip => "Data to be requested from IP",
            ComponentRateStatusType.Rateprovided or ComponentRateStatusType.Default => "Rate provided",
            _ => throw new IPUploadMappingException($"Cannot convert {componentRateStatusType} to string.")
        };
    }
}