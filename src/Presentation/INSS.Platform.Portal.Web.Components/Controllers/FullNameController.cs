using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class FullNameController : BaseController<FullNameModel>
{
    public FullNameController(IModelService<FullNameModel> fullNameService) : base(fullNameService)
    {          
    }
}