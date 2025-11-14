using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IJourneyService
{
    void TransitionPrevious(FormModel form, PageModel? pageModel = null);

    void TransitionNext(FormModel form, PageModel pageModel);
}