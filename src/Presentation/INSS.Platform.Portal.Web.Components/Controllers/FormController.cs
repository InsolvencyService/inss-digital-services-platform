using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Application.Validation;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class FormController : Controller
{
    private readonly IFormService _formService;
    private readonly IServiceProvider _serviceProvider;

    public FormController(IFormService formService, IServiceProvider serviceProvider)
    {
        _formService = formService;
        _serviceProvider = serviceProvider;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        BaseModel model = await _formService.GetAsync();
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Index([FromForm]BaseModel model)
    {
        IModelStateValidator modelStateValidator = GetModelValidator(model);
        await modelStateValidator.ValidateAsync(ModelState, model);
        
        if (!ModelState.IsValid)
        {
            return View("~/Views/Form/Index.cshtml", model);    
        }

        BaseModel nextPage = await _formService.SaveAsync(model);
        return Redirect(nextPage.PageUrl);
    }
    
    [HttpGet]
    public async Task<IActionResult> Start()
    {
        BaseModel firstPage = await _formService.StartAsync(Request.Path.Value!);
        return Redirect(firstPage.PageUrl);
    }
    
    [HttpGet]
    public async Task<IActionResult> Add(string itemId)
    {
        BaseModel page = await _formService.AddAsync(itemId);
        return Redirect(page.PageUrl);
    }
    
    [HttpGet]
    public async Task<IActionResult> Change(string itemId)
    {
        BaseModel page = await _formService.ChangeAsync(itemId);
        return Redirect(page.PageUrl);
    }
    
    [HttpGet]
    public async Task<IActionResult> Remove(string itemId)
    {
        ConfirmModel model = await _formService.RemoveAsync(itemId);
        return View("~/Views/Form/Index.cshtml", model);
    }
    
    private IModelStateValidator GetModelValidator(BaseModel page)
    {
        Type validatorType = typeof(IModelStateValidator<>).MakeGenericType(page.GetType());
        return (IModelStateValidator)(_serviceProvider.GetService(validatorType) ?? new DefaultModelStateValidator());
    }
}