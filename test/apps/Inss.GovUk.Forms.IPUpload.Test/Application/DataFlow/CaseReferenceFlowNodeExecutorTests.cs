using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
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
    public async Task CaseRefMatchesExisting_ExecuteAsync_ReturnsEmployerCheckNode()
    {
        CheckCaseReferenceModel checkCaseReference = _section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        checkCaseReference.CaseReference.Value = "CN12345678";
        CheckCaseReferenceModel currentPage = new() { Path = checkCaseReference.Path, CaseReference = { Value = "CN12345678" } };
        FlowNode currentNode = _nodes.First(n => n.PagePath == checkCaseReference.Path);
        FlowNodeContext context = CreateFlowNodeContext(currentPage, currentNode);
        
        NodeId? nextNodeId = await _caseReferenceFlowNodeExecutor.ExecuteAsync(context);

        EmployerDetailsModel employerDetails = _section.Pages.GetFirstOf<EmployerDetailsModel>();
        FlowNode employerDetailsNode = _nodes.First(n => n.PagePath == employerDetails.Path);
        Assert.Equal(employerDetailsNode.Id, nextNodeId);
    }
    
    [Fact]
    public async Task CaseRefMatchesExisting_ExecuteAsync_DoesNotResetSectionReturnPath()
    {
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        _section.ReturnUrl = summary.Path;
        CheckCaseReferenceModel checkCaseReference = _section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        checkCaseReference.CaseReference.Value = "CN12345678";
        CheckCaseReferenceModel currentPage = new() { Path = checkCaseReference.Path, CaseReference = { Value = "CN12345678" } };
        FlowNode currentNode = _nodes.First(n => n.PagePath == checkCaseReference.Path);
        FlowNodeContext context = CreateFlowNodeContext(currentPage, currentNode);
        
        await _caseReferenceFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(_section.ReturnUrl);
    }
    
    [Fact]
    public async Task CaseRefMatchesDiffers_ExecuteAsync_ResetsSectionReturnPath()
    {
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        _section.ReturnUrl = summary.Path;
        CheckCaseReferenceModel checkCaseReference = _section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        checkCaseReference.CaseReference.Value = "CN12345678";
        CheckCaseReferenceModel currentPage = new() { Path = checkCaseReference.Path, CaseReference = { Value = "CN87654321" } };
        FlowNode currentNode = _nodes.First(n => n.PagePath == checkCaseReference.Path);
        FlowNodeContext context = CreateFlowNodeContext(currentPage, currentNode);
        
        await _caseReferenceFlowNodeExecutor.ExecuteAsync(context);

        Assert.Null(_section.ReturnUrl);
    }
    
    private FlowNodeContext CreateFlowNodeContext(PageModel currentPage, FlowNode currentNode)
    {
        return new FlowNodeContext
        {
            Nodes = _nodes,
            CurrentNode = currentNode,
            Form = _form,
            Section = _section,
            CurrentPage = currentPage,
            PageBeforeChanges = _section.Pages.GetPage(currentPage.Path)
        };
    }
}