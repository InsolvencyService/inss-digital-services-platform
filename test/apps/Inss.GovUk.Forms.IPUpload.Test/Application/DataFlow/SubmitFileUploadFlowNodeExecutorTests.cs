using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class SubmitFileUploadFlowNodeExecutorTests
{
    private readonly SubmitFileUploadFlowNodeExecutor _submitFileUploadFlowNodeExecutor;
    private readonly ISubmitUploadedXmlService _submitUploadedXmlService;
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
        _submitUploadedXmlService = Substitute.For<ISubmitUploadedXmlService>();
        _submitFileUploadFlowNodeExecutor = new SubmitFileUploadFlowNodeExecutor(_submitUploadedXmlService);
    }

    [Fact]
    public async Task SubmittingSection_ExecuteAsync_SetsSectionCompleted()
    {
        await _submitFileUploadFlowNodeExecutor.ExecuteAsync(_context);
        
        Assert.NotNull(_section.CompletedDate);
    }
    
    [Fact]
    public async Task SubmittingSection_ExecuteAsync_UpdatesPostSubmitReferenceNumber()
    {
        const string referenceNumber = "J880ZFKY";
        _submitUploadedXmlService.SubmitAsync(_section, _form.Id).Returns(referenceNumber);
        
        await _submitFileUploadFlowNodeExecutor.ExecuteAsync(_context);

        PostSubmitModel postSubmit = _section.Pages.GetFirstOf<PostSubmitModel>();
        Assert.Equal(referenceNumber, postSubmit.ReferenceNumber);
    }
    
    [Fact]
    public async Task SubmittingSection_ExecuteAsync_ReturnsNextNode()
    {
        NodeId? nextNodeId = await _submitFileUploadFlowNodeExecutor.ExecuteAsync(_context);

        Assert.Equal(_node.NextNodes[0], nextNodeId);
    }
}