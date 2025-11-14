using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public class AddressService : BasePageModelService<AddressModel>
{
    public AddressService(
        IFormStateService formStateService, 
        IJourneyService  journeyService,
        IUserSessionResolver  userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
    }
    
    protected override void CopySourceToTargetModel(AddressModel sourceModel, AddressModel targetModel)
    {
        targetModel.AddressLine1 = sourceModel.AddressLine1;
        targetModel.AddressLine2 = sourceModel.AddressLine2;
        targetModel.TownCity = sourceModel.TownCity;
        targetModel.County = sourceModel.County;
        targetModel.Postcode = sourceModel.Postcode;
    }
}