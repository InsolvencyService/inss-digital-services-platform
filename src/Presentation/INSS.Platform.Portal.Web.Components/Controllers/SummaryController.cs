using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class SummaryController : BaseController<SectionModel>
{
    public SummaryController(IModelService<SectionModel> sectionService) : base(sectionService)
    {          
    }
}