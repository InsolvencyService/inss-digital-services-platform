namespace GovUk.Forms.Application.PageFlow;

public interface IPageExecutor
{
    ValueTask ExecuteAsync(ExecutePageContext context);
}