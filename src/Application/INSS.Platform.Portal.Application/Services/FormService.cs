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
    
    public async Task<BaseModel> GetAsync()
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

        return form.GetCurrentPage();
    }

    public async Task<BaseModel> StartAsync(string path)
    {
        FormModel form = await _formStateService.GetAsync();
        SectionModel section = form.GetSectionByUrl(path.Replace("/start", ""));
        BaseModel startPage = section.GetStartPage();
        form.Context.PreviousPageUrl = form.PageUrl;
        form.Context.CurrentPageId = startPage.Id;
        await this._formStateService.SaveAsync(form);
        return startPage;
    }
    
    public async Task<BaseModel> SaveAsync(BaseModel model)
    {
        FormModel form = await _formStateService.GetAsync();

        if (model is ConfirmModel confirm)
        {
            return await ProcessPostConfirmPageUrlAsync(form, confirm);
        }

        if (model is AddAnotherModel)
        {
            return form.GetNextPageAfter(model.Id);
        }
        
        AddAnotherModel? addAnother = form.FindAddAnother(model);

        if (addAnother is not null)
        {
            return await UpdateAddAnotherModelAsync(form, addAnother, model);
        }

        BaseModel currentModel = form.GetCurrentPage();

        switch (currentModel)
        {
            case SectionModel section:
                section.IsComplete = true;
                form.Context.CurrentPageId = form.Id;
                form.Context.PreviousPageUrl = null;
                break;
            default:
                model.CopyTo(currentModel);
                break;
        }

        BaseModel page = form.GetNextPageAfter(currentModel.Id);
        
        await _formStateService.SaveAsync(form);

        return page;
    }

    public async Task<BaseModel> AddAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        form.Context.CurrentPageId = itemId;

        if (form.GetCurrentPage() is not AddAnotherModel addAnother)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {itemId}.");
        }

        BaseModel firstPage = addAnother.CreateNewRow();
        
        form.Context.CurrentPageId = firstPage.Id;
        form.Context.PreviousPageUrl = addAnother.PageUrl; // TODO: Is this right?
        
        await this._formStateService.SaveAsync(form);
        
        return firstPage;
    }
    
    public async Task<BaseModel> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        form.Context.CurrentPageId = itemId;
        await this._formStateService.SaveAsync(form);
        return form.GetCurrentPage();
    }

    public async Task<ConfirmModel> RemoveAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        form.Context.CurrentPageId = itemId;
        BaseModel page = form.GetCurrentPage();
        AddAnotherModel? addAnother = form.FindAddAnother(page);

        if (addAnother is null)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {itemId}.");
        }
        
        await this._formStateService.SaveAsync(form);
        
        return new ConfirmModel { Id = page.Id, PageUrl = addAnother.PageUrl };
    }

    private async Task<BaseModel> ProcessPostConfirmPageUrlAsync(FormModel form, ConfirmModel confirm)
    {
        BaseModel page = form.GetCurrentPage();
        
        AddAnotherModel? addAnother = form.FindAddAnother(page);
            
        if (addAnother is null)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {confirm.Id}.");
        }
        
        BaseModel? nextPage;
        
        if (confirm.Confirmed)
        {
            BaseModel[] items = addAnother.Items.First(li => li.Any(i => i.Id == page.Id));
            
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

        form.Context.CurrentPageId = nextPage.Id;
        form.Context.PreviousPageUrl = nextPage.PageUrl; // TODO: Where does this go?
            
        await _formStateService.SaveAsync(form);

        return nextPage;
    }

    private async Task<BaseModel> UpdateAddAnotherModelAsync(FormModel form, AddAnotherModel addAnother, BaseModel currentModel)
    {
        foreach (BaseModel item in addAnother.Items.SelectMany(i => i))
        {
            if (item.Id == currentModel.Id)
            {
                currentModel.CopyTo(item);
                break;
            }
        }
        
        BaseModel nextPage = form.GetNextPageAfter(currentModel.Id);
            
        await this._formStateService.SaveAsync(form);

        return nextPage;
    }
}