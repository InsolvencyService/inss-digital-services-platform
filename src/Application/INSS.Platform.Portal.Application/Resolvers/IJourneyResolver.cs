using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Resolvers;

public interface IJourneyResolver<in TPageModel> : IJourneyResolver where TPageModel : PageModel
{
}

public interface IJourneyResolver
{
    PageModel? Resolve(FormModel form, PageModel pageModel);
}