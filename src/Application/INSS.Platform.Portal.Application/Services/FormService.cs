using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

// TODO: Refactor - use handlers??

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
            SummaryListModel summaryList2 = form.FindSummaryList(confirm.Id);
            
            if (confirm.Confirmed)
            {
                summaryList2.Items = summaryList2.Items.Where(i => i.Id != confirm.Id).ToArray();
                await _formStateService.SaveAsync(form);
            }

            if (summaryList2.Items.Length == 0)
            {
                BaseModel previousPage = form.FindPageBefore(summaryList2);
                return previousPage.PageUrl;
            }
            
            return summaryList2.PageUrl;
        }

        BaseModel currentModel = form.FindPage(model.PageUrl);

        if (currentModel is SectionModel section)
        {
            section.IsComplete = true;
        }
        else if (currentModel is not SummaryListModel)
        {
            model.CopyTo(currentModel);   
        }

        BaseModel page = form.GetNextPageAfter(currentModel.PageUrl);
        page.PreviousPageUrl = model.PageUrl;
        
        // See if the next page is a summary list. If so then we need to...
        if (page is SummaryListModel summaryList)
        {
            BaseModel? currentItem = summaryList.Items.FirstOrDefault(p => p.Id == currentModel.Id);

            if (currentItem is null)
            {
                // Add a copy
                summaryList.Items = summaryList.Items.Concat([currentModel.Clone()]).ToArray();
                currentModel.Reset();
            }
            else
            {
                // Replace
                currentModel.CopyTo(currentItem);
                currentModel.Reset();
            }
            
        }
        
        await _formStateService.SaveAsync(form);
        
        page.PreviousPageUrl = model.PageUrl;

        return page is SectionModel ? page.PageUrl + "/summary" : page.PageUrl;
    }

    public async Task<string> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        
        SummaryListModel summaryList = form.FindSummaryList(itemId);
        BaseModel page = summaryList.Items.First(i => i.Id == itemId);
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
}