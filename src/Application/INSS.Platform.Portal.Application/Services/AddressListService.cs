using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public class AddressListService : BasePageModelService<AddressListModel>
{
    public AddressListService(
        IFormStateService  formStateService, 
        IJourneyService  journeyService,
        IUserSessionResolver userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
    }

    protected override void CopySourceToTargetModel(AddressListModel sourceModel, AddressListModel targetModel)
    {
        targetModel.AddressList.AddRange(sourceModel.AddressList ?? Enumerable.Empty<AddressModel>());
    }
}