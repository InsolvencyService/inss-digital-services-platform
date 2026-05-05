using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowchartValidator
{
    private readonly FlowNode[] _nodes;

    public FlowchartValidator(FlowNode[] nodes)
    {
        _nodes = nodes;
    }
    
    public void Validate(SectionModel section, IServiceProvider serviceProvider)
    {
        List<string> flowchartErrors = [];
        
        if (_nodes.Length == 0)
        {
            flowchartErrors.Add("No nodes have been defined.");
        }
        else
        {
            ValidateNodePageAssociations(flowchartErrors, section);
            ValidateAddAnotherPageAssociations(flowchartErrors, section, serviceProvider);
            ValidateCheckAnswersPageAssociations(flowchartErrors, section, serviceProvider);
            ValidateAddAnotherWorkingPageAssociations(flowchartErrors, section, serviceProvider);
            ValidateSummaryNode(flowchartErrors, section, serviceProvider);
        }
        
        if (flowchartErrors.Count > 0)
        {
            throw new FlowchartException(string.Join(Environment.NewLine, flowchartErrors));
        }
    }
    
    private void ValidateNodePageAssociations(List<string> flowchartErrors, SectionModel section)
    {
        foreach (FlowNode node in _nodes)
        {
            PageModel? associatedPage = section.Pages.GetAllPathPages().FindPage(node.PagePath);

            if (associatedPage is null)
            {
                flowchartErrors.Add($"Unable to find the page in the section for path {node.PagePath}.");
            }
        }
    }
    
    private void ValidateAddAnotherPageAssociations(List<string> flowchartErrors, SectionModel section, IServiceProvider serviceProvider)
    {
        foreach (FlowNode node in _nodes)
        {
            PageModel? associatedPage = section.Pages.GetAllPathPages().FindPage(node.PagePath);

            if (associatedPage is not AddAnotherModel)
            {
                continue;
            }

            if (serviceProvider.GetKeyedService<IFlowNodeLoader>(node.Id) is not AddAnotherFlowNodeLoader)
            {
                flowchartErrors.Add(
                    $"Unable to find required add another loader for node with Id {node.Id} for page path {node.PagePath}.");
            }
            
            if (serviceProvider.GetKeyedService<IFlowNodeExecutor>(node.Id) is not AddAnotherFlowNodeExecutor)
            {
                flowchartErrors.Add(
                    $"Unable to find required add another executor for node with Id {node.Id} for page path {node.PagePath}.");
            }
        }
    }
    
    private void ValidateCheckAnswersPageAssociations(List<string> flowchartErrors, SectionModel section, IServiceProvider serviceProvider)
    {
        foreach (FlowNode node in _nodes)
        {
            PageModel? associatedPage = section.Pages.GetAllPathPages().FindPage(node.PagePath);

            if (associatedPage is not CheckAnswersModel)
            {
                continue;
            }

            if (serviceProvider.GetKeyedService<IFlowNodeLoader>(node.Id) is not CheckAnswersFlowNodeLoader)
            {
                flowchartErrors.Add(
                    $"Unable to find required check answers loader for node with Id {node.Id} for page path {node.PagePath}.");
            }
        }
    }
    
    private void ValidateAddAnotherWorkingPageAssociations(List<string> flowchartErrors, SectionModel section, IServiceProvider serviceProvider)
    {
        foreach (FlowNode node in _nodes)
        {
            PageModel? associatedPage = section.Pages.GetAllPathPages().FindPage(node.PagePath);
            
            if (associatedPage is CheckAnswersModel || 
                associatedPage is AddAnotherModel || 
                associatedPage is RemoveModel ||
                associatedPage?.MetaData.Group == GroupId.Empty)
            {
                continue;
            }
            
            if (serviceProvider.GetKeyedService<IFlowNodeExecutor>(node.Id) is not AddAnotherWorkingPageFlowNodeExecutor)
            {
                flowchartErrors.Add(
                    $"Unable to find required working page executor for node with Id {node.Id} for page path {node.PagePath}.");
            }
        }
    }
    
    private void ValidateSummaryNode(List<string> flowchartErrors, SectionModel section, IServiceProvider serviceProvider)
    {
        foreach (FlowNode node in _nodes)
        {
            PageModel? associatedPage = section.Pages.GetAllPathPages().FindPage(node.PagePath);

            if (associatedPage is not SummaryModel)
            {
                continue;
            }

            if (serviceProvider.GetKeyedService<IFlowNodeLoader>(node.Id) is not SectionSummaryFlowNodeLoader)
            {
                flowchartErrors.Add(
                    $"Unable to find required summary loader for node with Id {node.Id} for page path {node.PagePath}.");
            }
        }
    }
}