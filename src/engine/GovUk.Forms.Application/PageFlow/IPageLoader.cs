namespace GovUk.Forms.Application.PageFlow;

public interface IPageLoader
{
    ValueTask LoadAsync(LoadPageContext context);
}

public interface IPageExecutor
{
    ValueTask ExecuteAsync(ExecutePageContext context);
}