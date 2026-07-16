using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Inss.GovUk.Forms.Fip.Application.DataFlow;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.GovUk.Forms.Fip.Builders;

public sealed class FipFlowchart : DefineFlowchartBuilder
{
    // TODO: Align this with your actual journey and pages defined
    
    public override void Construct(IServiceCollection services)
    {
        NodeId searchTermId = "SearchTerm";
        NodeId searchResultId = "SearchResult";
        NodeId searchDetailId = "SearchDetail";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Find an Insolvency Practitioner"];
            
        SearchTermModel searchTerm = section.Pages.GetFirstOf<SearchTermModel>();
        SearchResultModel searchResult = section.Pages.GetFirstOf<SearchResultModel>();
        SearchResultDetailModel searchResultDetail = section.Pages.GetFirstOf<SearchResultDetailModel>();
            
        services.AddKeyedTransient<IFlowNodePreviousPathProvider, FlowNodePreviousPathProvider>(section.Path);
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(searchTermId, searchTerm.Path, searchResultId)
            .WithExecutor<SearchTermFlowNodeExecutor>()
            .Next()
            .AddSpurNode(searchResultId, searchResult.Path, searchResultId, searchDetailId)
            .WithLoader<SearchResultFlowNodeLoader>()
            .WithExecutor<SearchResultFlowNodeExecutor>()
            .Next()
            .AddEndNode(searchDetailId, searchResultDetail.Path, searchTermId)
            .WithLoader<SearchResultDetailFlowNodeLoader>()
            .BuildAndRegister();
    }
}