using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public class ConfirmService : BasePageModelService<ConfirmModel>
{
    public ConfirmService(
        IFormStateService  formStateService, 
        IJourneyService  journeyService,
        IUserSessionResolver userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
    }
    
    protected override void CopySourceToTargetModel(ConfirmModel sourceModel, ConfirmModel targetModel)
    {
        targetModel.ConfirmationId = sourceModel.ConfirmationId;
        targetModel.Confirm = sourceModel.Confirm;
    }
}