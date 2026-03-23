using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class DeclarationFlowNodeExecutorTests
{
    [Fact]
    public async Task ProcessingDeclaration_ExecuteAsync_SetsAcceptedTrue()
    {
        DeclarationFlowNodeExecutor executor = new();
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        IPUploadDeclarationModel declaration = ipUploadSection.Pages.GetFirstOf<IPUploadDeclarationModel>();
        declaration.Accepted = false;
        FlowNode node = new() { Id = "NodeId1", PagePath = declaration.Path, NextNodes = ["NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            UpdatedPage = new IPUploadDeclarationModel { Path = declaration.Path }
        };
        
        await executor.ExecuteAsync(context);
        
        Assert.True(declaration.Accepted);
    }
    
    [Fact]
    public async Task ProcessingDeclaration_ExecuteAsync_ReturnsNextNode()
    {
        DeclarationFlowNodeExecutor executor = new();
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        IPUploadDeclarationModel declaration = ipUploadSection.Pages.GetFirstOf<IPUploadDeclarationModel>();
        declaration.Accepted = false;
        FlowNode node = new() { Id = "NodeId1", PagePath = declaration.Path, NextNodes = ["NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            UpdatedPage = new IPUploadDeclarationModel { Path = declaration.Path }
        };
        
        NodeId? nextNodeId = await executor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[0], nextNodeId);
    }
}