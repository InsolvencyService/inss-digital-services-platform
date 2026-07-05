namespace GovUk.Forms.Application.PageFlow;

public interface IPageLoader
{
    ValueTask LoadAsync(LoadPageContext context);
}