using System.Text.Json;
using Inss.Common.IPUpload.Employee.Api;
using Inss.FormsSubmission.Service.IPUpload.Exceptions;

namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class RP14AApiMapper : IMapper
{
    private readonly RP14A _model;
    
    public RP14AApiMapper(RP14A model)
    {
        _model = model;
    }
    
    public JsonMessage[] Map()
    {
        EmployeeInformation[] employeeInformationList = _model.Employee.Select(e => new EmployeeInformation
        {
            CorrelationId = Guid.NewGuid(),
            CaseReference = _model.Header.CaseReference,
            EmployerName = _model.EmployerName,
            Employee = new Employee
            {
                Title = e.EmployeeName.Title,
                FirstNames = e.EmployeeName.Forenames,
                LastName = e.EmployeeName.Surname,
                NationalInsuranceNumber = e.NINO,
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
            .Select(e => new JsonMessage { CorrelationId = e.CorrelationId.ToString(), Json = JsonSerializer.Serialize(e) })
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
        return payDetails.ComponentPayPerWeek
            .Select(cop => new ComponentPayPerWeek
            {
                ComponentType = MapTransferType(cop.ComponentType)!,
                ComponentRate = cop.ComponentRateSpecified ? cop.ComponentRate : string.Empty,
                ComponentRateStatus = MapTransferStatus(cop.ComponentRateStatus)!
            })
            .ToList();
    }
    
    private static List<ArrearsOfPay> MapArrearsOfPayList(RP14AEmployeePayDetailsArrearsOfPayPeriod[] arrearsOfPayList)
    {
        return arrearsOfPayList
            .Select(aop => new ArrearsOfPay
            {
                StartDate = aop.Period.StartDate,
                EndDate = aop.Period.EndDate,
                AmountOwed = aop.AOPOwedSpecified ? aop.AOPOwed : null,
                Paytype = aop.PayTypeSpecified ? aop.PayType.ToString() : null!
            })
            .ToList();
    }

    private static List<TakenAndNotPaid> MapTakenAndNotPaidList(RP14AEmployeeHoliday holiday)
    {
        return holiday.HolidayNotPaid
            .Select(hnp => new TakenAndNotPaid
            {
                StartDate = hnp.StartDate,
                EndDate = hnp.EndDate
            })
            .ToList();
    }
    
    private static string? MapTransferType(RP14AEmployeePayDetailsComponentPayPerWeekComponentType? componentType)
    {
        if (!componentType.HasValue)
        {
            return null;
        }

        return componentType.Value switch
        {
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.ArrearsofPay => "Arrears of Pay",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.BasicAward => "Basic Award",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.CompensatoryNoticePay => "Compensatory Notice Pay",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.HolidayPayAccrued => "Holiday Pay Accrued",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.HolidayPayTakenNotPaid => "Holiday Pay Taken Not Paid",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.NoticeWorkedNotPaid => "Notice Worked Not Paid",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.ProtectiveAward => "Protective Award",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.RedundancyPay => "Redundancy Pay",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentType.RefundofNotionalTax => "Refund of Notional Tax",
            _ => throw new IPUploadMappingException($"Cannot convert {componentType} to string.")
        };
    }
    
    private static string? MapTransferStatus(RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus? componentRateStatusType)
    {
        if (!componentRateStatusType.HasValue)
        {
            return null;
        }

        return componentRateStatusType.Value switch
        {
            RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Datanotyetavailable => "Data not yet available",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Fixedrateofpay => "Fixed rate of pay",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Noholidaypaypayable => "No holiday pay payable",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Noratedefinedinlaw => "No rate defined in law",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Datatoberequestedfromip => "Data to be requested from IP",
            RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Rateprovided or 
                RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Default => "Rate provided",
            _ => throw new IPUploadMappingException($"Cannot convert {componentRateStatusType} to string.")
        };
    }
}