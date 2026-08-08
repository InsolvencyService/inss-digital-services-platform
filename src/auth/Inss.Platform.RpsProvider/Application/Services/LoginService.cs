using Inss.Platform.Application.Validators;
using Inss.Platform.Domain.Components.Common;
using Inss.Platform.Domain.Validation;
using Inss.Platform.RpsProvider.Application.Clients;
using Inss.Platform.RpsProvider.Domain.Enums;

namespace Inss.Platform.RpsProvider.Application.Services;

public sealed class LoginService : IPageValidator
{
    private readonly IUserAuthenticationPageClient _userAuthenticationPageClient;
    private readonly IUserAuthenticationClient _userAuthenticationClient;

    public LoginService(IUserAuthenticationPageClient userAuthenticationPageClient, IUserAuthenticationClient userAuthenticationClient)
    {
        _userAuthenticationPageClient = userAuthenticationPageClient;
        _userAuthenticationClient = userAuthenticationClient;
    }

    public async ValueTask ValidateAsync(PageContext context)
    {
        EmailComponentModel email = context.Page.Components.GetFirstOf<EmailComponentModel>();
        PasswordComponentModel password = context.Page.Components.GetFirstOf<PasswordComponentModel>();
        LoginResponse userLoginPageResponse = await _userAuthenticationPageClient.GetAsync();
        RpsAuthenticationTypes loginResult = await _userAuthenticationClient.AuthenticateAsync(
            email.Value!, password.Value!, userLoginPageResponse.CsrfToken);
        
        if (loginResult == RpsAuthenticationTypes.Unknown)
        {
            PageValidationError error = new()
            {
                Properties = ["Components[1].Value"],
                Message = "The email address or password you entered is incorrect"
            };
            context.Page.PageValidationInfo = new PageValidationInfo { Errors = [error] };
        }
        else if (loginResult == RpsAuthenticationTypes.Locked)
        {
            PageValidationError error = new()
            {
                Properties = ["Components[1].Value"],
                Message = "Your account is locked"
            };
            context.Page.PageValidationInfo = new PageValidationInfo { Errors = [error] };
        }
        else if (loginResult == RpsAuthenticationTypes.Outage)
        {
            PageValidationError error = new()
            {
                Properties = ["Components[1].Value"],
                Message = "There is an account login outage"
            };
            context.Page.PageValidationInfo = new PageValidationInfo { Errors = [error] };
        }
    }
}