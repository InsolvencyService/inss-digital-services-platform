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

        return form.FindPage(form.CurrentPageId);
    }

    public async Task<BaseModel> StartAsync(string path)
    {
        FormModel form = await _formStateService.GetAsync();
        form.History.Push(new NavigationItem(form.Id, form.PageUrl));
        SectionModel section = form.Sections.GetSectionByUrl(path.Replace("/start", ""));
        BaseModel startPage = section.GetStartPage();
        form.CurrentPageId = startPage.Id;
        await this._formStateService.SaveAsync(form);
        return startPage;
    }
    
    public async Task<BaseModel> SaveAsync(BaseModel model)
    {
        FormModel form = await _formStateService.GetAsync();

        switch (model)
        {
            case ConfirmModel confirm:
            {
                BaseModel nextPage = confirm.HandleConfirmation(form);
                await _formStateService.SaveAsync(form);
                return nextPage;
            }
            case AddAnotherModel { AddAnotherItem: true } addAnother:
            {
                AddAnotherModel currentAddAnother = form.GetAddAnother(addAnother.Id);
                BaseModel firstPage = currentAddAnother.Items.CreateNewRow();
                form.CurrentPageId = firstPage.Id;
                await _formStateService.SaveAsync(form);
                return firstPage;
            }
            default:
            break;
        }

        BaseModel currentPage = form.GetCurrentPageFor(model);
        
        switch (currentPage)
        {
            case SectionModel section:                                                                                                                                                                                
                section.IsComplete = true;
                form.History.Clear();
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
        
        if (currentPage is not SectionModel)
        {
            form.History.Push(new NavigationItem(currentPage.Id, currentPage.PageUrl));
        }

        await _formStateService.SaveAsync(form);

        return page;
    }

    public async Task<string> GoBackAsync()
    {
        FormModel form = await _formStateService.GetAsync();
        NavigationItem? navigationItem = form.History.Pop();

        if (navigationItem is not null)
        {
            form.CurrentPageId = navigationItem.PageId;
            await _formStateService.SaveAsync(form);
        }

        return navigationItem?.PageUrl ?? form.PageUrl;
    }

    public async Task<BaseModel> ChangeAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel currentPage = form.FindPage(form.CurrentPageId);
        form.History.Push(new NavigationItem(currentPage.Id, currentPage.PageUrl));
        await this._formStateService.SaveAsync(form);
        form.CurrentPageId = itemId;
        return form.FindPage(itemId);
    }

    public async Task<ConfirmModel> RemoveAsync(string itemId)
    {
        FormModel form = await _formStateService.GetAsync();
        BaseModel currentPage = form.FindPage(form.CurrentPageId);
        form.History.Push(new NavigationItem(currentPage.Id, currentPage.PageUrl));
        BaseModel page = form.FindPage(itemId);
        AddAnotherModel addAnother = form.Sections.GetAddAnotherFor(page);
        await this._formStateService.SaveAsync(form);
        return new ConfirmModel { Id = page.Id, PageUrl = addAnother.PageUrl };
    }
}