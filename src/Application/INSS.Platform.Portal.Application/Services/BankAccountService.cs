using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Services;

public class BankAccountService : BasePageModelService<BankAccountModel>
{
    private readonly IHttpClientFactory _clientFactory;

    public BankAccountService(
        IHttpClientFactory clientFactory, 
        IFormStateService  formStateService, 
        IJourneyService  journeyService,
        IUserSessionResolver userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
        _clientFactory = clientFactory;
    }
    
    protected override async Task ValidateAdditionalAsync(ModelStateDictionary modelState, BankAccountModel model)
    {
        HttpClient client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri("https://vseries.bottomline.com/api/");

        HttpResponseMessage response = await client.GetAsync($"getukbankbranch/?apikey=2T2-2E42AEF5-3CF8-4FD9-B1A5-09A9BF03551D&sortCode={model.SortCode}");

        if (!response.IsSuccessStatusCode || await response.Content.ReadAsStringAsync() == "null")
        {
            modelState.AddModelError(nameof(model.SortCode), "Bank account sort code not found");
        }
    }

    protected override void CopySourceToTargetModel(BankAccountModel sourceModel, BankAccountModel targetModel)
    {
        targetModel.AccountNumber = sourceModel.AccountNumber;
        targetModel.SortCode = sourceModel.SortCode;
    }
}