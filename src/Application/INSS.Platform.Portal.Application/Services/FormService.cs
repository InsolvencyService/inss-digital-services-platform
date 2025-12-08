using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public sealed class FormService : IFormService
{
    private readonly IFormStateService _formStateService;
    private readonly IFormModelFactory _formModelFactory;

    public FormService(IFormStateService formStateService, IFormModelFactory formModelFactory)
    {
        _formStateService = formStateService;
        _formModelFactory = formModelFactory;
    }
    
    public async Task<BaseModel> GetAsync(string path)
    {
        FormModel form;
        
        if (!await _formStateService.FormExistsAsync())
        {
            form = await _formModelFactory.CreateAsync();
            await _formStateService.SaveAsync(form);
        }
        else
        {
            form = await _formStateService.GetAsync();
        }
        
        return form.FindPage(path);
    }

    public async Task<string> SaveAsync(BaseModel model)
    {
        FormModel form = await _formStateService.GetAsync();

        if (model is ConfirmModel confirm)
        {
            return await GetPostComfirmPageUrlAsync(form, confirm);
        }

        BaseModel currentModel = form.FindPage(model.PageUrl);

        switch (currentModel)
        {
            case SectionModel section:
                section.IsComplete = true;
                break;
            case SummaryListModel:
                // No action required
                break;
            default:
                model.CopyTo(currentModel);
                break;
        }

        BaseModel page = form.GetNextPageAfter(currentModel.PageUrl);
        page.PreviousPageUrl = model.PageUrl;
        
        if (page is SummaryListModel summaryList)
        {
            UpdateSummaryListModel(summaryList, currentModel);
        }
        
        await _formStateService.SaveAsync(form);
        
        page.PreviousPageUrl = model.PageUrl;

        return page is SectionModel ? page.PageUrl + "/summary" : page.PageUrl;
    }

    public async Task<string> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        SummaryListModel summaryList = form.FindSummaryList(itemId);
        BaseModel page = summaryList.Items.Single(i => i.Id == itemId);
        BaseModel previousPage = form.FindPageBefore(summaryList);
        page.CopyTo(previousPage);
        previousPage.Id = page.Id;

        await _formStateService.SaveAsync(form);
        
        return previousPage.PageUrl;
    }

    public async Task<ConfirmModel> RemoveAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();

        SummaryListModel summaryList = form.FindSummaryList(itemId);

        return new ConfirmModel { Id = itemId, PageUrl = summaryList.PageUrl };
    }

    private async Task<string> GetPostComfirmPageUrlAsync(FormModel form, ConfirmModel confirm)
    {
        SummaryListModel summaryList = form.FindSummaryList(confirm.Id);
            
        if (confirm.Confirmed)
        {
            summaryList.Items = summaryList.Items.Where(i => i.Id != confirm.Id).ToArray();
            await _formStateService.SaveAsync(form);
        }

        if (summaryList.Items.Length == 0)
        {
            BaseModel previousPage = form.FindPageBefore(summaryList);
            return previousPage.PageUrl;
        }
        
        return summaryList.PageUrl;
    }

    private static void UpdateSummaryListModel(SummaryListModel summaryList, BaseModel currentModel)
    {
        BaseModel? currentItem = summaryList.Items.FirstOrDefault(p => p.Id == currentModel.Id);

        if (currentItem is null)
        {
            // Add a copy
            summaryList.Items = [.. summaryList.Items, currentModel.Clone()];
            currentModel.Reset();
        }
        else
        {
            // Replace
            currentModel.CopyTo(currentItem);
            currentModel.Reset();
        }
    }
}