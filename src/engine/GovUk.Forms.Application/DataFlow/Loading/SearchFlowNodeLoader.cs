using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SearchFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();
        search.CurrentResult = null;
        
        // TODO: Handle result detail by setting it on the search model 
        if (context.State is not null)
        {
            // Find result and set to search.CurrentResult
        }
        // The context has a state with will be the Id for the result so you can find it and set the CurrentResult
        
        return new ValueTask<NodeId?>((NodeId?)null);
    }
}