using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class FormController : BaseController<FormModel>
{
    public FormController(IModelService<FormModel> modelService) : base(modelService)
    {
    }
}