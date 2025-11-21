using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Web.Models;

namespace INSS.Platform.Portal.Web.Services;

public class HomeValueService : BasePageModelService<HomeValueModel>
{
    public HomeValueService(
        IFormStateService formStateService,
        IJourneyService  journeyService,
        IUserSessionResolver  userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
    }

    protected override void CopySourceToTargetModel(HomeValueModel sourceModel, HomeValueModel targetModel)
    {
        targetModel.Value = sourceModel.Value;
    }
}