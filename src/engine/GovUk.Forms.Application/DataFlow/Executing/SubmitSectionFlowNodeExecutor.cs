using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SubmitSectionFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserSessionProvider _userSessionProvider;
    private const int CompletedIndex = 0;

    public SubmitSectionFlowNodeExecutor(IServiceProvider serviceProvider, IUserSessionProvider userSessionProvider)
    {
        _serviceProvider = serviceProvider;
        _userSessionProvider = userSessionProvider;
    }

    public async ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        if (context.FinalExecuteStep)
        {
            context.Section.State = SectionStateTypes.Completed;

            foreach (PageModel page in context.Section.Pages)
            {
                page.EditMode |= PageEditTypes.Locked;
                page.PreviousPagePath = context.Form.Path;
            }
            
            string userSessionId = await _userSessionProvider.ResolveAsync();
            ISubmitSectionService submitSectionService =
                _serviceProvider.GetRequiredKeyedService<ISubmitSectionService>(context.Section.Path);
            await submitSectionService.SubmitAsync(context.Section, userSessionId);
            
            return context.CurrentNode.NextNodes.Length == 1 ? context.CurrentNode.NextNodes[CompletedIndex] : null;
        }

        return null;
    }
}