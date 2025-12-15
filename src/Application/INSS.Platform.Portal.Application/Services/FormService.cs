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
    
    public async Task<(BaseModel, NavigationItem?)> GetAsync(string path)
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

        if (form.History.IsLastEntry(path))
        {
            NavigationItem item = form.History.Pop()!;
            form.CurrentPageId = item.PageId;
        }
        
        BaseModel page = form.FindPage(form.CurrentPageId);

        await this._formStateService.SaveAsync(form);
        
        return (page, form.History.Peek());
    }

    public async Task<BaseModel> StartAsync(string path)
    {
        FormModel form = await _formStateService.GetAsync();
        SectionModel section = form.Sections.GetSectionByUrl(path.Replace("/start", ""));
        BaseModel startPage = section.GetStartPage();
        form.CurrentPageId = startPage.Id;
        form.History.Clear();
        form.History.Push(new NavigationItem(form.Id, form.PageUrl));
        await this._formStateService.SaveAsync(form);
        return startPage;
    }
    
    public async Task<BaseModel> SaveAsync(BaseModel model)
    {
        FormModel form = await _formStateService.GetAsync();

        if (model is ConfirmModel confirm)
        {
            BaseModel nextPage = confirm.HandleConfirmation(form);
            await _formStateService.SaveAsync(form);
            return nextPage;
        }
        
        BaseModel currentPage = form.GetCurrentPageFor(model);
        
        switch (currentPage)
        {
            case SectionModel section:
                section.IsComplete = true;
                break;
            default:
            {
                if (currentPage is not AddAnotherModel)
                {
                    model.CopyTo(currentPage);    
                }

                break;
            }
        }
        
        BaseModel page = form.FindNextPageAfter(currentPage);

        form.CurrentPageId = page.Id;

        if (currentPage is SectionModel)
        {
            form.History.Clear();
        }
        else
        {
            form.History.Push(new NavigationItem(currentPage.Id, currentPage.PageUrl));
        }

        await _formStateService.SaveAsync(form);

        return page;
    }

    public async Task<BaseModel> AddAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        AddAnotherModel addAnother = form.GetAddAnother(itemId);
        BaseModel firstPage = addAnother.Items.CreateNewRow();
        form.CurrentPageId = firstPage.Id;
        await this._formStateService.SaveAsync(form);
        return firstPage;
    }
    
    public async Task<BaseModel> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        await this._formStateService.SaveAsync(form);
        form.CurrentPageId = itemId;
        return form.FindPage(itemId);
    }

    public async Task<ConfirmModel> RemoveAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel page = form.FindPage(itemId);
        AddAnotherModel addAnother = form.Sections.GetAddAnotherFor(page);
        await this._formStateService.SaveAsync(form);
        return new ConfirmModel { Id = page.Id, PageUrl = addAnother.PageUrl };
    }
}