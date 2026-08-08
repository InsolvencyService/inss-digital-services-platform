using Inss.Platform.Application.Factories;
using Inss.Platform.Component;
using Inss.Platform.Component.Builders;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.RpsProvider.Application.Services;
using Inss.Platform.RpsProvider.Options;

namespace Inss.Platform.RpsProvider;

public sealed class RpsAppBuilder : AppBuilder
{
    public override PagePath[] Build(IServiceCollection services, IConfiguration configuration)
    {
        LoginOptions loginOptions = configuration.BindAndValidate<LoginOptions>("Login");
        
        PageModel loginPage = PageModelBuilder
            .For("Login", "/login", "Sign in")
            .PreviousPageIs(loginOptions.BackUrl)
            .NextPageIs(loginOptions.BackUrl)
            .WithPageValidator<LoginService>()
            .AddHeadingComponent("Sign in")
            .ComponentAdded()
            .AddEmailComponent("Email address")
            .WithRequiredValidator("Enter an email address")
            .WithEmailValidator("Enter an email address in the correct format, like name@example.com")
            .ComponentAdded()
            .AddPasswordComponent("Password")
            .WithRequiredValidator("Enter a password")
            .ComponentAdded()
            .AddLinkComponent("Forgot password", loginOptions.ForgotPasswordUrl)
            .ComponentAdded()
            .Build(services);
        
        services.AddSingleton<IAppFactory>(_ => new AppFactory([loginPage]));
        
        return [loginPage.Path];
    }
}