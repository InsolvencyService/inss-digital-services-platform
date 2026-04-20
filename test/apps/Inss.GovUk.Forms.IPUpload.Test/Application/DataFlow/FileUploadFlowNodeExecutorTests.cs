using System.Text;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadFlowNodeExecutorTests
{
    private readonly FileUploadFlowNodeExecutor _fileUploadFlowNodeExecutor;
    
    public FileUploadFlowNodeExecutorTests()
    {
        _fileUploadFlowNodeExecutor = new FileUploadFlowNodeExecutor();
    }
    
    [Fact]
    public async Task FirstPassExecution_ExecuteAsync_ReturnsSummaryNodeId()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        XmlFileUploadModel ipUpload = ipUploadSection.Pages.GetFirstOf<XmlFileUploadModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = ipUpload.Path, NextNodes = ["NodeId2", "NodeId3"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            FinalExecuteStep = false,
            UpdatedPage = new XmlFileUploadModel { Contents = Convert.ToBase64String(Encoding.UTF8.GetBytes(RP14AXmlNoErrors)) }
        };

        NodeId? nextNodeId = await _fileUploadFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal("NodeId3", nextNodeId);
    }
    
    [Fact]
    public async Task FinalPassExecutionNoErrors_ExecuteAsync_ReturnsSummaryNodeId()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        XmlFileUploadModel ipUpload = ipUploadSection.Pages.GetFirstOf<XmlFileUploadModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = ipUpload.Path, NextNodes = ["NodeId2", "NodeId3"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            FinalExecuteStep = true,
            UpdatedPage = new XmlFileUploadModel { Contents = Convert.ToBase64String(Encoding.UTF8.GetBytes(RP14AXmlNoErrors)) }
        };

        NodeId? nextNodeId = await _fileUploadFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal("NodeId3", nextNodeId);
    }

    [Fact]
    public async Task FinalPassExecutionWithErrors_ExecuteAsync_ReturnsSummaryNodeId()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        XmlFileUploadModel ipUpload = ipUploadSection.Pages.GetFirstOf<XmlFileUploadModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = ipUpload.Path, NextNodes = ["NodeId2", "NodeId3"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            FinalExecuteStep = true,
            UpdatedPage = new XmlFileUploadModel { Contents = Convert.ToBase64String(Encoding.UTF8.GetBytes(RP14AXmlWithErrors)) }
        };

        NodeId? nextNodeId = await _fileUploadFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal("NodeId2", nextNodeId);
    }
    
    [Fact]
    public async Task FinalPassExecutionWithErrors_ExecuteAsync_PopulatesPageWithErrorInfo()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        XmlFileUploadModel ipUpload = ipUploadSection.Pages.GetFirstOf<XmlFileUploadModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = ipUpload.Path, NextNodes = ["NodeId2", "NodeId3"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            FinalExecuteStep = true,
            UpdatedPage = new XmlFileUploadModel { Contents = Convert.ToBase64String(Encoding.UTF8.GetBytes(RP14AXmlWithErrors)) }
        };

        await _fileUploadFlowNodeExecutor.ExecuteAsync(context);

        IPUploadXmlErrorsModel ipUploadErrors = ipUploadSection.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        Assert.True(ipUploadErrors.HasErrors);
        Assert.NotEmpty(ipUploadErrors.GetErrors("Case"));
        Assert.NotEmpty(ipUploadErrors.GetErrors("Employee"));
        Assert.Empty(ipUploadErrors.GetErrors("Employer"));
        Assert.Empty(ipUploadErrors.GetErrors("Employee Pay"));
        Assert.Empty(ipUploadErrors.GetErrors("Employee Holiday"));
        Assert.Empty(ipUploadErrors.GetErrors("Employee Holiday Not Paid"));
    }
    
    private const string RP14AXmlWithErrors = """
        <?xml version="1.0" standalone="yes"?>
        <ns1:RP14A xmlns:ns1="http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application">
          <ns1:Employee>
            <ns1:Header>
              <ns1:CaseReference>XN10000112</ns1:CaseReference>
            </ns1:Header>
            <ns1:EmployerName>BANGLA FISH BAZAAR LIMITED</ns1:EmployerName>
            <ns1:EmployeeName>
              <ns1:Surname>Edmondson</ns1:Surname>
              <ns1:Forenames>Adrian</ns1:Forenames>
              <ns1:Title>Mr</ns1:Title>
            </ns1:EmployeeName>
            <ns1:NIClass>C</ns1:NIClass>
            <ns1:NINO>BP0752C</ns1:NINO>
            <ns1:DateOfBirth>1963-06-10</ns1:DateOfBirth>
            <ns1:StartDate>2017-01-03</ns1:StartDate>
            <ns1:DateNoticeGiven>2020-09-09</ns1:DateNoticeGiven>
            <ns1:EndDate>2020-09-09</ns1:EndDate>
            <ns1:PayDetails>
              <ns1:BasicPayPerWeek>500</ns1:BasicPayPerWeek>
              <ns1:WeeklyPayDay>Saturday</ns1:WeeklyPayDay>
              <ns1:ArrearsOfPay>
                <ns1:ArrearsOfPayPeriod1>
                  <ns1:AOP1StartDate>2020-08-01</ns1:AOP1StartDate>
                  <ns1:AOP1EndDate>2020-08-31</ns1:AOP1EndDate>
                  <ns1:AOPOwed1>2100</ns1:AOPOwed1>
                  <ns1:AOPPayType1>bouncedcheque</ns1:AOPPayType1>
                </ns1:ArrearsOfPayPeriod1>
                <ns1:ArrearsOfPayPeriod2 />
                <ns1:ArrearsOfPayPeriod3 />
                <ns1:ArrearsOfPayPeriod4 />
              </ns1:ArrearsOfPay>
            </ns1:PayDetails>
            <ns1:Holiday>
              <ns1:HolidayNotPaid>
                <ns1:Holiday1 />
                <ns1:Holiday2 />
                <ns1:Holiday3 />
              </ns1:HolidayNotPaid>
            </ns1:Holiday>
          </ns1:Employee>
        </ns1:RP14A>
        """;
    
    private const string RP14AXmlNoErrors = """
       <?xml version="1.0" standalone="yes"?>
       <ns1:RP14A xmlns:ns1="http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application">
         <ns1:Employee>
           <ns1:Header>
             <ns1:CaseReference>CN10000112</ns1:CaseReference>
           </ns1:Header>
           <ns1:EmployerName>BANGLA FISH BAZAAR LIMITED</ns1:EmployerName>
           <ns1:EmployeeName>
             <ns1:Surname>Edmondson</ns1:Surname>
             <ns1:Forenames>Adrian</ns1:Forenames>
             <ns1:Title>Mr</ns1:Title>
           </ns1:EmployeeName>
           <ns1:NIClass>C</ns1:NIClass>
           <ns1:NINO>BP011752C</ns1:NINO>
           <ns1:DateOfBirth>1963-06-10</ns1:DateOfBirth>
           <ns1:StartDate>2017-01-03</ns1:StartDate>
           <ns1:DateNoticeGiven>2020-09-09</ns1:DateNoticeGiven>
           <ns1:EndDate>2020-09-09</ns1:EndDate>
           <ns1:PayDetails>
             <ns1:BasicPayPerWeek>500</ns1:BasicPayPerWeek>
             <ns1:WeeklyPayDay>Saturday</ns1:WeeklyPayDay>
             <ns1:ArrearsOfPay>
               <ns1:ArrearsOfPayPeriod1>
                 <ns1:AOP1StartDate>2020-08-01</ns1:AOP1StartDate>
                 <ns1:AOP1EndDate>2020-08-31</ns1:AOP1EndDate>
                 <ns1:AOPOwed1>2100</ns1:AOPOwed1>
                 <ns1:AOPPayType1>bouncedcheque</ns1:AOPPayType1>
               </ns1:ArrearsOfPayPeriod1>
               <ns1:ArrearsOfPayPeriod2 />
               <ns1:ArrearsOfPayPeriod3 />
               <ns1:ArrearsOfPayPeriod4 />
             </ns1:ArrearsOfPay>
           </ns1:PayDetails>
           <ns1:Holiday>
             <ns1:HolidayNotPaid>
               <ns1:Holiday1 />
               <ns1:Holiday2 />
               <ns1:Holiday3 />
             </ns1:HolidayNotPaid>
           </ns1:Holiday>
         </ns1:Employee>
       </ns1:RP14A>
       """;
}