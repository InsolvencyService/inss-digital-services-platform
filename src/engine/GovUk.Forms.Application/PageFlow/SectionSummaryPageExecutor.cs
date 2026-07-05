namespace GovUk.Forms.Application.PageFlow;

public sealed class SectionSummaryPageExecutor : IPageExecutor
{
    public ValueTask ExecuteAsync(ExecutePageContext context)
    {
        context.Section.SetCompleted();
        return ValueTask.CompletedTask;
    }
}