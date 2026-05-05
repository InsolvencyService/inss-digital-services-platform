using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class SubmitFileUploadFlowNodeExecutorTests
{
    private readonly SubmitFileUploadFlowNodeExecutor _submitFileUploadFlowNodeExecutor;
    private readonly ISubmitIPUploadSectionClient _submitIPUploadSectionClient;
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private readonly FlowNode _node;
    private readonly ExecuteContext _context;

    public SubmitFileUploadFlowNodeExecutorTests()
    {
        _form = TestFormModels.CreateWithIPUploadSection();
        _section = _form.Sections["IP Upload"];
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        _node = new FlowNode { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        _context = new ExecuteContext
        {
            Nodes = [_node],
            CurrentNode = _node,
            Form = _form,
            Section = _section,
            UpdatedPage = new IPUploadDeclarationModel { Path = summary.Path }
        };
        _submitIPUploadSectionClient = Substitute.For<ISubmitIPUploadSectionClient>();
        _submitFileUploadFlowNodeExecutor = new SubmitFileUploadFlowNodeExecutor(_submitIPUploadSectionClient);
    }

    [Fact]
    public async Task SubmittingSection_ExecuteAsync_SetsSectionCompleted()
    {
        await _submitFileUploadFlowNodeExecutor.ExecuteAsync(_context);
        
        Assert.NotNull(_section.CompletedDate);
    }
    
    [Fact]
    public async Task SubmittingSection_ExecuteAsync_CallsDynamicsClient()
    {
        await _submitFileUploadFlowNodeExecutor.ExecuteAsync(_context);

        await _submitIPUploadSectionClient.Received(1).SubmitAsync(_section, _form.Id);
    }
    
    [Fact]
    public async Task SubmittingSection_ExecuteAsync_ReturnsNextNode()
    {
        NodeId? nextNodeId = await _submitFileUploadFlowNodeExecutor.ExecuteAsync(_context);

        Assert.Equal(_node.NextNodes[0], nextNodeId);
    }
}