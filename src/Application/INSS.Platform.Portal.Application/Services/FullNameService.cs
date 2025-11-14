using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Services;

public class FullNameService : BasePageModelService<FullNameModel>
{
    public FullNameService(
        IFormStateService  formStateService, 
        IJourneyService  journeyService,
        IUserSessionResolver userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
    }
    
    protected override Task ValidateAdditionalAsync(ModelStateDictionary modelState, FullNameModel model)
    {
        // Any additional validation logic for FullNameModel can be added here if needed.
        return Task.CompletedTask;
    }

    protected override void CopySourceToTargetModel(FullNameModel sourceModel, FullNameModel targetModel)
    {
        targetModel.FullName = sourceModel.FullName;
    }
}