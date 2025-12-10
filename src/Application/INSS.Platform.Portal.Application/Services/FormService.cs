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
        
        BaseModel page = form.FindPage(path);

        // if (page is AddAnotherModel addAnother)
        // {
        //     addAnother.PageIndex ??= 0;
        //
        //     await _formStateService.SaveAsync(form);
        //     
        //     return addAnother.Pages[addAnother.PageIndex.Value];
        // }

        return page;
    }

    public async Task<string> SaveAsync(BaseModel model)
    {
        FormModel form = await _formStateService.GetAsync();

        if (model is ConfirmModel confirm)
        {
            return await GetPostConfirmPageUrlAsync2(form, confirm);
        }
        
        // Find out if the model belongs to the add another
        
        // If the model exists in the items then we are editing.
        
        // If it doesn't then we are adding
        
        // In all cases, reset the pages reference and then save form changes
        
        AddAnotherModel? addAnother = form.FindAddAnother(model);

        if (addAnother is not null)
        {
            bool updated = UpdateAddAnotherModel(addAnother, model);

            
            
            BaseModel page2 = form.FindPageById(model.Id);
            BaseModel nextPage = addAnother.GetNextPage(page2);
            
            page2.Reset();
            
            await this._formStateService.SaveAsync(form);
            
            return updated ? addAnother.PageUrl : nextPage.PageUrl;
        }
        
        
        
        
        
        

        BaseModel currentModel = form.FindPage(model.PageUrl);

        switch (currentModel)
        {
            case SectionModel section:
                section.IsComplete = true;
                break;
            //case SummaryListModel:
            //    // No action required
            //    break;
            default:
                model.CopyTo(currentModel);
                break;
        }

        BaseModel page = form.GetNextPageAfter(currentModel.PageUrl);
        page.PreviousPageUrl = model.PageUrl;
        
        // if (page is SummaryListModel summaryList)
        // {
        //     UpdateSummaryListModel(summaryList, currentModel);
        // }
        
        await _formStateService.SaveAsync(form);
        
        page.PreviousPageUrl = model.PageUrl;

        return page is SectionModel ? page.PageUrl + "/summary" : page.PageUrl;
    }

    public async Task<string> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel page2 = form.FindPageById2(itemId);
        AddAnotherModel? addAnother = form.FindAddAnother(page2);

        //BaseModel pageToEdit;
        
        foreach (BaseModel pageToEdit in addAnother!.Pages)
        {
            if (pageToEdit.GetType() == page2.GetType())
            {
                //pageToEdit = x;
                page2.CopyTo(pageToEdit);
                pageToEdit.Id = page2.Id;
                pageToEdit.PreviousPageUrl = addAnother.PageUrl;
                //pageToEdit.PageUrl = page2.PageUrl;

                await _formStateService.SaveAsync(form);
                
                return pageToEdit.PageUrl;
            }
        }

        throw new InvalidOperationException("Todo");
        //return pageToEdit.PageUrl;

        // SummaryListModel summaryList = form.FindSummaryList(itemId);
        // BaseModel page = summaryList.Items.Single(i => i.Id == itemId);
        // BaseModel previousPage = form.FindPageBefore(summaryList);
        // page.CopyTo(previousPage);
        // previousPage.Id = page.Id;
        //
        // await _formStateService.SaveAsync(form);
        //
        // return previousPage.PageUrl;
    }

    public async Task<ConfirmModel> RemoveAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel page2 = form.FindPageById2(itemId);
        AddAnotherModel? addAnother = form.FindAddAnother(page2);

        foreach (BaseModel pageToRemove in addAnother!.Pages)
        {
            if (pageToRemove.GetType() == page2.GetType())
            {
                return new ConfirmModel
                {
                    Id = itemId, 
                    PageUrl = addAnother.PageUrl
                };
            }
        }

        throw new InvalidOperationException("Todo");
    }

    private async Task<string> GetPostConfirmPageUrlAsync(FormModel form, ConfirmModel confirm)
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

    private async Task<string> GetPostConfirmPageUrlAsync2(FormModel form, ConfirmModel confirm)
    {
        BaseModel page2 = form.FindPageById2(confirm.Id);
        AddAnotherModel? addAnother = form.FindAddAnother(page2)!;
            
        if (confirm.Confirmed)
        {
            BaseModel[] list = addAnother.Items.First(i => i.Any(i2 => i2.Id == page2.Id));
            addAnother.Items.Remove(list);
            //addAnother.Items = summaryList.Items.Where(i => i.Id != confirm.Id).ToArray();
            await _formStateService.SaveAsync(form);
        }

        if (addAnother.Items.Count == 0)
        {
            return addAnother.GetFirstPage().PageUrl;
        }
        
        return addAnother.PageUrl;
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

    private static bool UpdateAddAnotherModel(AddAnotherModel addAnother, BaseModel currentModel)
    {
        BaseModel? existingModel = null;
        
        foreach (BaseModel[] items in addAnother.Items)
        {
            foreach (BaseModel item in items)
            {
                if (item.Id == currentModel.Id)
                {
                    existingModel = item;
                    break;
                }
            }

            if (existingModel is not null)
            {
                break;
            }
        }
        
        if (existingModel is not null)
        {
            currentModel.CopyTo(existingModel);
            //currentModel.Reset();
        }
        else
        {
            var y = addAnother.Items.LastOrDefault();
            
            if (y is null || y.Length == addAnother.Pages.Length)
            {
                addAnother.Items.Add([]);
            }
        
            int currentIndex = addAnother.Items.Count - 1;
        
            if (addAnother.Items[currentIndex].Length < addAnother.Pages.Length)
            {
                addAnother.Items[currentIndex] = [.. addAnother.Items[currentIndex], currentModel.Clone()];
                //currentModel.Reset();
                //currentModel.Id = Guid.NewGuid().ToString("D");
            }
        }

        return existingModel is not null;


        /*
        foreach (BaseModel[] x in addAnother.Items)
        {
            foreach (BaseModel y in x)
            {
                if (y.Id == currentModel.Id)
                {
                    currentModel.CopyTo(y);
                    currentModel.Reset();
                    return;
                }
            }
        }

        if (addAnother.Items.Count == 0)
        {
            addAnother.Items = new List<BaseModel[]>([[currentModel.Clone()]]);

            //addAnother.Items[0] = [.. addAnother.Items[0], currentModel.Clone()];
            currentModel.Reset();
            return;
        }

        int numPages = addAnother.Pages.Length;

        for (int i = 0; i < addAnother.Items.Count; i++)
        {
            if (addAnother.Items[i].Length < numPages)
            {
                addAnother.Items[i] = [.. addAnother.Items[i], currentModel.Clone()];
                break;
            }
        }
        */
        //addAnother.Items = [.. addAnother.Items, currentModel.Clone()];
        //currentModel.Reset();
        /*BaseModel? currentItem = addAnother.Items.FirstOrDefault(p => p.Id == currentModel.Id);

        if (currentItem is null)
        {
            // Add a copy
            addAnother.Items = [.. addAnother.Items, currentModel.Clone()];
            currentModel.Reset();
        }
        else
        {
            // Replace
            currentModel.CopyTo(currentItem);
            currentModel.Reset();
        }*/
    }
}