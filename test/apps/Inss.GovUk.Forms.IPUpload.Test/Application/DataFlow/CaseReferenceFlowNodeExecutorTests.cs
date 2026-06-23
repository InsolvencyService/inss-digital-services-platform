using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Application.Factories;
using Inss.GovUk.Forms.IPUpload.Builders;
using Inss.GovUk.Forms.IPUpload.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class CaseReferenceFlowNodeExecutorTests
{
    private readonly CaseReferenceFlowNodeExecutor _caseReferenceFlowNodeExecutor;
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private readonly FlowNode[] _nodes;
    
    public CaseReferenceFlowNodeExecutorTests()
    {
        IPUploadFormFactory formFactory = new();
        _form = formFactory.Create();
        _section = _form.Sections["IP Upload"];
        
        ServiceCollection services = [];
        services.AddSingleton(Substitute.For<ILogger<Flowchart>>());
        services.AddSingleton<IFormFactory>(formFactory);
        
        IPUploadFlowchart flowchartBuilder = new();
        flowchartBuilder.Construct(services);
        
        IFlowchart flowchart = services.BuildServiceProvider().GetRequiredKeyedService<IFlowchart>(_section.Path);
        _nodes = flowchart.Nodes;
        
        _caseReferenceFlowNodeExecutor = new CaseReferenceFlowNodeExecutor();
    }
    
    [Fact]
    public async Task NoCaseRefChangeAndPreviousPageWasSummary_ExecuteAsync_DoesNotResetReturn()
    {
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        CheckCaseReferenceModel checkCaseReference = _section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        checkCaseReference.CaseReference.Value = "CN12345678";
        checkCaseReference.ReturnUrl = summary.Path;
        checkCaseReference.LinkedToNode = _nodes.First(n => n.PagePath == checkCaseReference.Path).Id;
        EmployerDetailsModel employerDetails = _section.Pages.GetFirstOf<EmployerDetailsModel>();
        employerDetails.CaseReference = "CN12345678";
        employerDetails.EmployerName = "Springfield Nuclear";
        employerDetails.ReturnUrl = summary.Path;
        employerDetails.LinkedToNode = _nodes.First(n => n.PagePath == employerDetails.Path).Id;
        XmlFileUploadModel fileUpload = _section.Pages.GetFirstOf<XmlFileUploadModel>();
        fileUpload.Filename = "Test.xml";
        fileUpload.Contents = "<xml.>";
        fileUpload.ReturnUrl = summary.Path;
        fileUpload.LinkedToNode = _nodes.First(n => n.PagePath == fileUpload.Path).Id;
        FlowNode currentNode = _nodes.First(n => n.PagePath == checkCaseReference.Path);
        _section.Track(checkCaseReference.LinkedToNode);
        _section.Track(employerDetails.LinkedToNode);
        _section.Track(fileUpload.LinkedToNode);
        CheckCaseReferenceModel currentPage = new() { Path = checkCaseReference.Path, CaseReference = { Value = "CN12345678" } };
        FlowNodeContext context = CreateFlowNodeContext(currentPage, currentNode);
        
        await _caseReferenceFlowNodeExecutor.ExecuteAsync(context);
        
        Assert.Equal(summary.Path, checkCaseReference.ReturnUrl);
        Assert.Equal(summary.Path, employerDetails.ReturnUrl);
        Assert.Equal(summary.Path, fileUpload.ReturnUrl);
    }
    
    [Fact]
    public async Task HasCaseRefChangeAndPreviousPageWasSummary_ExecuteAsync_ResetsDetails()
    {
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        CheckCaseReferenceModel checkCaseReference = _section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        checkCaseReference.CaseReference.Value = "CN12345678";
        checkCaseReference.ReturnUrl = summary.Path;
        checkCaseReference.LinkedToNode = _nodes.First(n => n.PagePath == checkCaseReference.Path).Id;
        EmployerDetailsModel employerDetails = _section.Pages.GetFirstOf<EmployerDetailsModel>();
        employerDetails.CaseReference = "CN12345678";
        employerDetails.EmployerName = "Springfield Nuclear";
        employerDetails.ReturnUrl = summary.Path;
        employerDetails.LinkedToNode = _nodes.First(n => n.PagePath == employerDetails.Path).Id;
        XmlFileUploadModel fileUpload = _section.Pages.GetFirstOf<XmlFileUploadModel>();
        fileUpload.Filename = "Test.xml";
        fileUpload.Contents = "<xml.>";
        fileUpload.ReturnUrl = summary.Path;
        fileUpload.LinkedToNode = _nodes.First(n => n.PagePath == fileUpload.Path).Id;
        FlowNode currentNode = _nodes.First(n => n.PagePath == checkCaseReference.Path);
        _section.Track(checkCaseReference.LinkedToNode);
        _section.Track(employerDetails.LinkedToNode);
        _section.Track(fileUpload.LinkedToNode);
        CheckCaseReferenceModel currentPage = new() { Path = checkCaseReference.Path, CaseReference = { Value = "CN87654321" } };
        FlowNodeContext context = CreateFlowNodeContext(currentPage, currentNode);
        
        await _caseReferenceFlowNodeExecutor.ExecuteAsync(context);
        
        Assert.Null(checkCaseReference.ReturnUrl);
        Assert.Null(employerDetails.ReturnUrl);
        Assert.Null(fileUpload.ReturnUrl);
        Assert.Empty(fileUpload.Filename);
        Assert.Empty(fileUpload.Contents);
        Assert.Equal(0, fileUpload.Length);
    }
    
    [Fact]
    public async Task HasCaseRefChangeAndPreviousPageWasSummary_ExecuteAsync_ResetsReturn()
    {
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        CheckCaseReferenceModel checkCaseReference = _section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        checkCaseReference.CaseReference.Value = "CN12345678";
        checkCaseReference.ReturnUrl = summary.Path;
        FlowNode currentNode = _nodes.First(n => n.PagePath == checkCaseReference.Path);
        CheckCaseReferenceModel currentPage = new() { Path = checkCaseReference.Path, CaseReference = { Value = "CN87654321" } };
        FlowNodeContext context = CreateFlowNodeContext(currentPage, currentNode);
        
        await _caseReferenceFlowNodeExecutor.ExecuteAsync(context);
        
        Assert.Null(checkCaseReference.ReturnUrl);
    }
    
    private FlowNodeContext CreateFlowNodeContext(PageModel currentPage, FlowNode currentNode)
    {
        return new FlowNodeContext
        {
            Nodes = _nodes,
            CurrentNode = currentNode,
            Form = _form,
            Section = _section,
            CurrentPage = currentPage
        };
    }
}