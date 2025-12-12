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
        
        return form.GetPageByUrl(path);
    }

    public async Task<string> SaveAsync(BaseModel model)
    {
        FormModel form = await _formStateService.GetAsync();

        if (model is ConfirmModel confirm)
        {
            return await ProcessPostConfirmPageUrlAsync(form, confirm);
        }

        if (model is AddAnotherModel)
        {
            BaseModel nextPage = form.GetNextPageAfter(model.PageUrl);
            return nextPage.PageUrl;
        }
        
        AddAnotherModel? addAnother = form.FindAddAnother(model);

        if (addAnother is not null)
        {
            UpdateAddAnotherModel(addAnother, model);

            BaseModel nextPage = form.GetNextPageAfter(model.PageUrl);
            
            await this._formStateService.SaveAsync(form);

            return nextPage.PageUrl;
        }

        BaseModel currentModel = form.GetPageByUrl(model.PageUrl);

        switch (currentModel)
        {
            case SectionModel section:
                section.IsComplete = true;
                break;
            default:
                model.CopyTo(currentModel);
                break;
        }

        BaseModel page = form.GetNextPageAfter(currentModel.PageUrl);
        
        await _formStateService.SaveAsync(form);

        return page is SectionModel ? page.PageUrl + "/summary" : page.PageUrl;
    }

    public async Task<string> AddAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();

        if (form.GetPageById(itemId) is not AddAnotherModel addAnother)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {itemId}.");
        }

        BaseModel firstPage = addAnother.CreateNewRow();

        SectionModel section = form.GetSectionForPageId(firstPage.Id);

        section.Context.CurrentPageId = firstPage.Id;
        section.Context.PreviousPageUrl = addAnother.PageUrl; // TODO: Is this right?
        
        await this._formStateService.SaveAsync(form);
        
        return firstPage.PageUrl;
    }
    
    public async Task<string> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel page = form.GetPageById(itemId);
        AddAnotherModel? addAnother = form.FindAddAnother(page);
        SectionModel section = form.GetSectionForPageId(page.Id);
        
        if (addAnother is null)
        {
            section.Context.CurrentPageId = page.Id;
            section.Context.PreviousPageUrl = section.PageUrl;
            return page.PageUrl;
        }
        
        section.Context.CurrentPageId = page.Id;
        section.Context.PreviousPageUrl = addAnother.PageUrl;
        return page.PageUrl;
    }

    public async Task<ConfirmModel> RemoveAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel page = form.GetPageById(itemId);
        AddAnotherModel? addAnother = form.FindAddAnother(page);

        if (addAnother is null)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {itemId}.");
        }
        
        return new ConfirmModel
        {
            Id = itemId, 
            PageUrl = addAnother.PageUrl
        };
    }

    private async Task<string> ProcessPostConfirmPageUrlAsync(FormModel form, ConfirmModel confirm)
    {
        BaseModel page = form.GetPageById(confirm.Id);
        
        AddAnotherModel? addAnother = form.FindAddAnother(page);
            
        if (addAnother is null)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {confirm.Id}.");
        }

        SectionModel section = form.GetSectionForPageId(page.Id);
        
        BaseModel? nextPage;
        
        if (confirm.Confirmed)
        {
            BaseModel[] items = addAnother.Items.First(i => i.Any(i2 => i2.Id == page.Id));
            
            if (addAnother.Items.Count > 1)
            {
                addAnother.Items.Remove(items);
                nextPage = addAnother;
            }
            else
            {
                foreach (BaseModel item in items)
                {
                    item.Reset();
                }   
            
                nextPage = addAnother.Items[0][0];
            }
        }
        else
        {
            nextPage = addAnother;
        }

        section.Context.CurrentPageId = nextPage.Id;
        section.Context.PreviousPageUrl = nextPage.PageUrl; // TODO: Where does this go?
            
        await _formStateService.SaveAsync(form);

        return nextPage.PageUrl;
    }

    private static void UpdateAddAnotherModel(AddAnotherModel addAnother, BaseModel currentModel)
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
            var lastItem = addAnother.Items.LastOrDefault();
            
            if (lastItem is null)
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
        }
    }
}