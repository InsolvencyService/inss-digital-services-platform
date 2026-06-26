using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow;

public class FlowchartValidatorTests
{
    private readonly ServiceCollection _services;
    private readonly NodeId _fullNameNodeId = "FullName";
    private readonly NodeId _ageNodeId = "Age";
    private readonly NodeId _checkAnswersNodeId = "CheckAnswers";
    private readonly NodeId _addAnotherNodeId = "AddAnother";
    private readonly NodeId _removeNodeId = "Remove";
    private readonly NodeId _addressNodeId = "Address";
    private readonly NodeId _salaryNodeId = "Salary";
    private readonly NodeId _bankAccountNodeId = "BankAccount";
    private readonly NodeId _summaryNodeId = "Summary";
    
    public FlowchartValidatorTests()
    {
        _services = [];
    }
    
    [Fact]
    public void NoNodesDefined_Validate_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        FlowchartValidator flowchartValidator = new([]);
        
        FlowchartException exception = Assert.Throws<FlowchartException>(
            () => flowchartValidator.Validate(section, _services.BuildServiceProvider()));
        
        Assert.Contains("No nodes have been defined.", exception.Message);
    }

    [Fact]
    public void PageNotFoundForNode_Validate_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        FlowNode unknownNode = new() { Id = "NodeId1", PagePath = "/example/section/page", NextNodes = ["NodeId2"] };
        FlowchartValidator flowchartValidator = new([unknownNode]);
        
        FlowchartException exception = Assert.Throws<FlowchartException>(
            () => flowchartValidator.Validate(section, _services.BuildServiceProvider()));
        
        Assert.Contains("Unable to find the page in the section for path /example/section/page.", exception.Message);
    }
    
    [Fact]
    public void MissingWorkingPageExecutor_Validate_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();
        PageModelList allPages = section.Pages.GetAllPathPages();
        FlowNode fullNameNode = CreateFlowNode(_fullNameNodeId, allPages[0].Path, _ageNodeId);
        FlowNode ageNode = CreateFlowNode(_ageNodeId, allPages[1].Path, _checkAnswersNodeId);
        FlowNode checkAnswersNode = CreateFlowNode(_checkAnswersNodeId, allPages[2].Path, _addAnotherNodeId);
        FlowNode addAnotherNode = CreateFlowNode(_addAnotherNodeId, allPages[4].Path, _fullNameNodeId, _summaryNodeId);
        FlowNode removeNode = CreateFlowNode(_removeNodeId, allPages[3].Path, _addAnotherNodeId);
        FlowNode summaryNode = CreateFlowNode(_summaryNodeId, allPages[5].Path);
        FlowchartValidator flowchartValidator = new([fullNameNode, ageNode, checkAnswersNode, removeNode, addAnotherNode, summaryNode]);
        
        FlowchartException exception = Assert.Throws<FlowchartException>(
            () => flowchartValidator.Validate(section, _services.BuildServiceProvider()));
        
        Assert.Contains(
            $"Unable to find required working page executor for node with Id {_fullNameNodeId} for page path {allPages[0].Path}.", exception.Message);
    }
    
    [Fact]
    public void MissingAddAnotherLoader_Validate_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();
        PageModelList allPages = section.Pages.GetAllPathPages();
        FlowNode fullNameNode = CreateFlowNode(_fullNameNodeId, allPages[0].Path, _ageNodeId);
        FlowNode ageNode = CreateFlowNode(_ageNodeId, allPages[1].Path, _checkAnswersNodeId);
        FlowNode checkAnswersNode = CreateFlowNode(_checkAnswersNodeId, allPages[2].Path, _addAnotherNodeId);
        FlowNode addAnotherNode = CreateFlowNode(_addAnotherNodeId, allPages[4].Path, _fullNameNodeId, _summaryNodeId);
        FlowNode removeNode = CreateFlowNode(_removeNodeId, allPages[3].Path, _addAnotherNodeId);
        FlowNode summaryNode = CreateFlowNode(_summaryNodeId, allPages[5].Path);
        FlowchartValidator flowchartValidator = new([fullNameNode, ageNode, checkAnswersNode, removeNode, addAnotherNode, summaryNode]);
        
        FlowchartException exception = Assert.Throws<FlowchartException>(
            () => flowchartValidator.Validate(section, _services.BuildServiceProvider()));
        
        Assert.Contains(
            $"Unable to find required add another loader for node with Id {_addAnotherNodeId} for page path {allPages[4].Path}.", exception.Message);
    }
    
    [Fact]
    public void MissingCheckAnswersLoader_Validate_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();
        PageModelList allPages = section.Pages.GetAllPathPages();
        FlowNode fullNameNode = CreateFlowNode(_fullNameNodeId, allPages[0].Path, _ageNodeId);
        FlowNode ageNode = CreateFlowNode(_ageNodeId, allPages[1].Path, _checkAnswersNodeId);
        FlowNode checkAnswersNode = CreateFlowNode(_checkAnswersNodeId, allPages[2].Path, _addAnotherNodeId);
        FlowNode addAnotherNode = CreateFlowNode(_addAnotherNodeId, allPages[4].Path, _fullNameNodeId, _summaryNodeId);
        FlowNode removeNode = CreateFlowNode(_removeNodeId, allPages[3].Path, _addAnotherNodeId);
        FlowNode summaryNode = CreateFlowNode(_summaryNodeId, allPages[5].Path);
        FlowchartValidator flowchartValidator = new([fullNameNode, ageNode, checkAnswersNode, removeNode, addAnotherNode, summaryNode]);
        
        FlowchartException exception = Assert.Throws<FlowchartException>(
            () => flowchartValidator.Validate(section, _services.BuildServiceProvider()));
        
        Assert.Contains($"Unable to find required check answers loader for node with " +
                        $"Id {_checkAnswersNodeId} for page path {allPages[2].Path}.", exception.Message);
    }
    
    [Fact]
    public void MissingAddAnotherExecutor_Validate_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();
        PageModelList allPages = section.Pages.GetAllPathPages();
        FlowNode fullNameNode = CreateFlowNode(_fullNameNodeId, allPages[0].Path, _ageNodeId);
        FlowNode ageNode = CreateFlowNode(_ageNodeId, allPages[1].Path, _checkAnswersNodeId);
        FlowNode checkAnswersNode = CreateFlowNode(_checkAnswersNodeId, allPages[2].Path, _addAnotherNodeId);
        FlowNode addAnotherNode = CreateFlowNode(_addAnotherNodeId, allPages[4].Path, _fullNameNodeId, _summaryNodeId);
        FlowNode removeNode = CreateFlowNode(_removeNodeId, allPages[3].Path, _addAnotherNodeId);
        FlowNode summaryNode = CreateFlowNode(_summaryNodeId, allPages[5].Path);
        FlowchartValidator flowchartValidator = new([fullNameNode, ageNode, checkAnswersNode, removeNode, addAnotherNode, summaryNode]);
        
        FlowchartException exception = Assert.Throws<FlowchartException>(
            () => flowchartValidator.Validate(section, _services.BuildServiceProvider()));
        
        Assert.Contains($"Unable to find required add another executor for node with " +
                        $"Id {_addAnotherNodeId} for page path {allPages[4].Path}.", exception.Message);
    }
    
    [Fact]
    public void ValidYourDetailsFlowchart_Validate_DoesNotThrowException()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        FlowNode fullNameNode = CreateFlowNode(_fullNameNodeId, section.Pages[0].Path, _addressNodeId);
        FlowNode addressNode = CreateFlowNode(_addressNodeId, section.Pages[1].Path, _ageNodeId, _salaryNodeId);
        FlowNode ageNode = CreateFlowNode(_ageNodeId, section.Pages[2].Path, _bankAccountNodeId);
        FlowNode salaryNode = CreateFlowNode(_salaryNodeId, section.Pages[3].Path, _bankAccountNodeId);
        FlowNode bankAccountNode = CreateFlowNode(_bankAccountNodeId, section.Pages[4].Path, _summaryNodeId);
        FlowNode summaryNode = CreateFlowNode(_summaryNodeId, section.Pages[5].Path);
        _services.AddKeyedSingleton<IFlowNodeLoader>(summaryNode.Id, (_, _) => Substitute.For<SummaryFlowNodeLoader>());
        FlowchartValidator flowchartValidator = new([fullNameNode, addressNode, ageNode, salaryNode, bankAccountNode, summaryNode]);
        
        try
        {
            flowchartValidator.Validate(section, _services.BuildServiceProvider());
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }

    [Fact]
    public void ValidAddAnotherFlowchart_Validate_DoesNotThrowException()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();
        PageModelList allPages = section.Pages.GetAllPathPages();
        FlowNode fullNameNode = CreateFlowNode(_fullNameNodeId, allPages[0].Path, _ageNodeId);
        FlowNode ageNode = CreateFlowNode(_ageNodeId, allPages[1].Path, _checkAnswersNodeId);
        FlowNode checkAnswersNode = CreateFlowNode(_checkAnswersNodeId, allPages[2].Path, _addAnotherNodeId);
        FlowNode addAnotherNode = CreateFlowNode(_addAnotherNodeId, allPages[4].Path, _fullNameNodeId, _summaryNodeId);
        FlowNode removeNode = CreateFlowNode(_removeNodeId, allPages[3].Path, _addAnotherNodeId);
        FlowNode summaryNode = CreateFlowNode(_summaryNodeId, allPages[5].Path);
        _services.AddKeyedSingleton<IFlowNodeExecutor, AddAnotherWorkingPageFlowNodeExecutor>(fullNameNode.Id);
        _services.AddKeyedSingleton<IFlowNodeExecutor, AddAnotherWorkingPageFlowNodeExecutor>(ageNode.Id);
        _services.AddKeyedSingleton<IFlowNodeLoader, CheckAnswersFlowNodeLoader>(checkAnswersNode.Id);
        _services.AddKeyedSingleton<IFlowNodeLoader, AddAnotherFlowNodeLoader>(addAnotherNode.Id);
        _services.AddKeyedSingleton<IFlowNodeExecutor, AddAnotherFlowNodeExecutor>(addAnotherNode.Id);
        _services.AddKeyedSingleton<IFlowNodeLoader, AddAnotherRemoveFlowNodeLoader>(removeNode.Id);
        _services.AddKeyedSingleton<IFlowNodeExecutor, AddAnotherRemoveFlowNodeExecutor>(removeNode.Id);
        _services.AddKeyedSingleton<IFlowNodeLoader>(summaryNode.Id, (_, _) => Substitute.For<SummaryFlowNodeLoader>());
        FlowchartValidator flowchartValidator = new([fullNameNode, ageNode, checkAnswersNode, removeNode, addAnotherNode, summaryNode]);
        
        try
        {
            flowchartValidator.Validate(section, _services.BuildServiceProvider());
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }
    
    private static FlowNode CreateFlowNode(NodeId nodeId, ContentPath pagePath, params NodeId[] nextNodeIds)
    {
        return new FlowNode { Id = nodeId, PagePath = pagePath, NextNodes = nextNodeIds };
    }
}