using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Domain.Exceptions;

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
            return await ProcessPostConfirmPageUrlAsync(form, confirm);
        }
        
        AddAnotherModel? addAnother = form.FindAddAnother(model);

        if (addAnother is not null)
        {
            bool updated = UpdateAddAnotherModel(addAnother, model);

            
            
            //BaseModel page2 = form.FindPageById(model.Id);
            BaseModel nextPage = addAnother.GetNextPage(model.Id);
            
            //page2.Reset();
            
            await this._formStateService.SaveAsync(form);

            return nextPage.PageUrl;//updated ? addAnother.PageUrl : nextPage.PageUrl;
        }
        
        
        
        
        
        

        BaseModel currentModel = form.FindPage(model.PageUrl);

        switch (currentModel)
        {
            case SectionModel section:
                //section.IsComplete = true;
                section.Progress = SectionProgressType.Completed;
                break;
            default:
                model.CopyTo(currentModel);
                break;
        }

        BaseModel page = form.GetNextPageAfter(currentModel.PageUrl);
        page.PreviousPageUrl = model.PageUrl;
        
        await _formStateService.SaveAsync(form);
        
        page.PreviousPageUrl = model.PageUrl;

        return page is SectionModel ? page.PageUrl + "/summary" : page.PageUrl;
    }

    public async Task<string> AddAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();

        if (form.FindPageById2(itemId) is not AddAnotherModel addAnother)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {itemId}.");
        }

        BaseModel firstPage = addAnother.CreateNewRow();

        await this._formStateService.SaveAsync(form);
        
        return firstPage.PageUrl;
    }
    
    public async Task<string> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel page2 = form.FindPageById2(itemId);
        AddAnotherModel? addAnother = form.FindAddAnother(page2);

        if (addAnother is null)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {itemId}.");
        }

        addAnother.CurrentAction = AddAnotherActionMode.Edit;
        addAnother.CurrentEditId = itemId;
        
        int index = -1;

        foreach (BaseModel[] items in addAnother.Items)
        {
            index++;
            bool found = false;
            
            foreach (BaseModel item in items)
            {
                if (item.Id == itemId)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                break;
            }
        }
        
        // for (int i = 0; i < addAnother.Items[index].Length; i++)
        // {
        //     addAnother.Items[index][i].CopyTo(addAnother.Pages[i]);
        //     //addAnother.Items[index][i].CopyTo(addAnother.Pages[i]);
        //     addAnother.Pages[i].Id = addAnother.Items[index][i].Id;
        //     addAnother.Pages[i].PageUrl = addAnother.Items[index][i].PageUrl;
        // }

        return addAnother.Items[index][0].PageUrl;
        
        /*foreach (BaseModel pageToEdit in addAnother!.Pages)
        {
            if (pageToEdit.GetType() == page2.GetType())
            {
                page2.CopyTo(pageToEdit);
                pageToEdit.Id = page2.Id;
                pageToEdit.PreviousPageUrl = addAnother.PageUrl;

                await _formStateService.SaveAsync(form);
                
                return pageToEdit.PageUrl;
            }
        }*/

        //throw new InvalidOperationException("Todo");
    }

    public async Task<ConfirmModel> RemoveAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel page2 = form.FindPageById2(itemId);
        AddAnotherModel? addAnother = form.FindAddAnother(page2);

        if (addAnother is null)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {itemId}.");
        }
        
        addAnother.CurrentAction = AddAnotherActionMode.Remove;
        
        foreach (BaseModel[] items in addAnother.Items)
        {
            foreach (BaseModel item in items)
            {
                if (item.Id == itemId)
                {
                    return new ConfirmModel
                    {
                        Id = itemId, 
                        PageUrl = addAnother.PageUrl
                    };    
                }
            }
            // if (pageToRemove.GetType() == page2.GetType())
            // {
            //     return new ConfirmModel
            //     {
            //         Id = itemId, 
            //         PageUrl = addAnother.PageUrl
            //     };
            // }
        }

        throw new InvalidOperationException("Todo");
    }

    private async Task<string> ProcessPostConfirmPageUrlAsync(FormModel form, ConfirmModel confirm)
    {
        BaseModel page2 = form.FindPageById2(confirm.Id);
        
        AddAnotherModel? addAnother = form.FindAddAnother(page2);
            
        if (addAnother is null)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {confirm.Id}.");
        }

        BaseModel? nextPage = null;
        
        if (confirm.Confirmed)
        {
            BaseModel[] list = addAnother.Items.First(i => i.Any(i2 => i2.Id == page2.Id));
            
            if (addAnother.Items.Count > 1)
            {
                addAnother.Items.Remove(list);
                nextPage = addAnother;
                addAnother.CurrentAction = AddAnotherActionMode.Summary;
                //addAnother.CurrentAction = AddAnotherActionMode.Summary;
            }
            else
            {
                foreach (BaseModel item in list)
                {
                    item.Reset();
                }   
            
                nextPage = addAnother.Items[0][0];//.GetNextPage(confirm.Id);
                addAnother.CurrentAction = AddAnotherActionMode.Edit;
                addAnother.CurrentEditId = nextPage.Id;
            }
            
            
            
            await _formStateService.SaveAsync(form);
        }
        else
        {
            nextPage = addAnother;
        }

        // if (addAnother.Items.Count == 0)
        // {
        //     return addAnother.GetFirstPage().PageUrl;
        // }

        return nextPage.PageUrl;//addAnother.GetNextPage(confirm.Id).PageUrl;
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
        }
        else
        {
            var y = addAnother.Items.LastOrDefault();
            
            if (y is null) // || y.Length == addAnother.Pages.Length)
            {
                addAnother.Items.Add([]);
            }
        
            int currentIndex = addAnother.Items.Count - 1;

            foreach (BaseModel item in addAnother.Items[currentIndex])
            {
                if (item.GetType() == currentModel.GetType())
                {
                    currentModel.CopyTo(item);
                    break;
                }
            }
            // if (addAnother.Items[currentIndex].Length < addAnother.Pages.Length)
            // {
            //     addAnother.Items[currentIndex] = [.. addAnother.Items[currentIndex], currentModel.Clone()];
            // }
        }

        return existingModel is not null;
    }
}