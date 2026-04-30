namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14aBuilder
{
    private readonly string _caseReference = "CN10000112";
    private string _nino = "BP011752C";
    // private string _moneyOwed = "100";

    public Rp14aBuilder WithNationalInsuranceNumber(string nino)
    {
        _nino = nino;
        return this;
    }

    //public Rp14aBuilder WithMoneyOwedToEmployer(string moneyOwed)
    //{
    //    _moneyOwed = moneyOwed;
    //    return this;
    //}

    public string Build()
    {
        return $"""
        <ns1:RP14A xmlns:ns1="www.inss.gsi.gov.uk/RP14A_Application">
        <ns1:Header>
        <ns1:CaseReference>{_caseReference}</ns1:CaseReference>
        </ns1:Header>
        <ns1:EmployerName>BANGLA FISH BAZAAR LIMITED</ns1:EmployerName>
        <ns1:Employee>
        <ns1:EmployeeName>
        <ns1:Surname>Edmondson</ns1:Surname>
        <ns1:Forenames>Adrian</ns1:Forenames>
        <ns1:Title>Mr</ns1:Title>
        </ns1:EmployeeName>
        <ns1:NIClass>C</ns1:NIClass>
        <ns1:NINO>{_nino}</ns1:NINO>
        <ns1:DateOfBirth>1963-06-10</ns1:DateOfBirth>
        <ns1:StartDate>2017-01-03</ns1:StartDate>
        <ns1:DateNoticeGiven>2020-09-09</ns1:DateNoticeGiven>
        <ns1:EndDate>2020-09-09</ns1:EndDate>
        <ns1:PayDetails>
        <ns1:BasicPayPerWeek>500</ns1:BasicPayPerWeek>
        <ns1:WeeklyPayDay>Saturday</ns1:WeeklyPayDay>
        <ns1:ArrearsOfPay>
        <ns1:ArrearsOfPayPeriod>
        <ns1:Period>
        <ns1:StartDate>2020-08-01</ns1:StartDate>
        <ns1:EndDate>2020-08-31</ns1:EndDate>
        </ns1:Period>
        <ns1:AOPOwed>2100</ns1:AOPOwed>
        <ns1:PayType>bouncedcheque</ns1:PayType>
        </ns1:ArrearsOfPayPeriod>
        </ns1:ArrearsOfPay>
        </ns1:PayDetails>
        <ns1:Holiday>
        <ns1:HolidayNotPaid>
        <ns1:Holiday>
        <ns1:StartDate>2020-08-01</ns1:StartDate>
        <ns1:EndDate>2020-08-31</ns1:EndDate>
        </ns1:Holiday>
        </ns1:HolidayNotPaid>
        </ns1:Holiday>
        </ns1:Employee>
        </ns1:RP14A>
        """;
    }
}
