using System.Text;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadFlowNodeExecutorTests
{
    private readonly FileUploadFlowNodeExecutor _fileUploadFlowNodeExecutor;
    private readonly IValidationFactory _validationFactory;
    
    public FileUploadFlowNodeExecutorTests()
    {
        _validationFactory = Substitute.For<IValidationFactory>();
        _fileUploadFlowNodeExecutor = new FileUploadFlowNodeExecutor(_validationFactory);
    }
    
    [Fact]
    public async Task NoErrors_ExecuteAsync_ReturnsSummaryNodeId()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        EmployerDetailsModel employerDetails = ipUploadSection.Pages.GetFirstOf<EmployerDetailsModel>();
        XmlFileUploadModel ipUpload = ipUploadSection.Pages.GetFirstOf<XmlFileUploadModel>();
        IBaseValidator validator = Substitute.For<IBaseValidator>();
        validator.Validate(employerDetails).Returns(new EmployeeValidatorContext());
        _validationFactory.Create(Arg.Any<object>()).Returns(validator);
        
        FlowNode node = new() { Id = "NodeId1", PagePath = ipUpload.Path, NextNodes = ["NodeId2", "NodeId3"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            CurrentPage = new XmlFileUploadModel { Contents = Convert.ToBase64String(Encoding.UTF8.GetBytes(RP14AXmlNoErrors)) }
        };

        NodeId? nextNodeId = await _fileUploadFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal("NodeId3", nextNodeId);
    }

    [Fact]
    public async Task WithErrors_ExecuteAsync_ReturnsSummaryNodeId()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        EmployerDetailsModel employerDetails = ipUploadSection.Pages.GetFirstOf<EmployerDetailsModel>();
        XmlFileUploadModel ipUpload = ipUploadSection.Pages.GetFirstOf<XmlFileUploadModel>();
        IBaseValidator validator = Substitute.For<IBaseValidator>();
        var validatorContext = new EmployeeValidatorContext();
        validatorContext.AddError(new ValidationInfo(), "Error");
        validator.Validate(employerDetails).Returns(validatorContext);
        _validationFactory.Create(Arg.Any<object>()).Returns(validator);
        FlowNode node = new() { Id = "NodeId1", PagePath = ipUpload.Path, NextNodes = ["NodeId2", "NodeId3"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            CurrentPage = new XmlFileUploadModel { Contents = Convert.ToBase64String(Encoding.UTF8.GetBytes(RP14AXmlWithErrors)) }
        };

        NodeId? nextNodeId = await _fileUploadFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal("NodeId2", nextNodeId);
    }
  
    private const string RP14AXmlWithErrors = """
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