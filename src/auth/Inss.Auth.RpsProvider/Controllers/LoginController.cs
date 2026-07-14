using System.Security.Claims;
using GovUk.Forms.Application.Providers;
using Inss.Auth.RpsProvider.Application.Services;
using Inss.Auth.RpsProvider.Domain.Enums;
using Inss.Auth.RpsProvider.Models;
using Inss.Auth.RpsProvider.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Inss.Auth.RpsProvider.Controllers;

public class LoginController : Controller
{
    private readonly ILoginService _loginService;
    private readonly IOptions<LoginOptions> _loginOptions;
    private readonly IPagePropertiesProvider _pagePropertiesProvider;

    public LoginController(ILoginService  loginService, IOptions<LoginOptions> loginOptions, IPagePropertiesProvider pagePropertiesProvider)
    {
        _loginService = loginService;
        _loginOptions = loginOptions;
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        _pagePropertiesProvider.PreviousPagePath = _loginOptions.Value.BackUrl;
        return View(new LoginModel{ ReturnUrl = Request.Query["returnUrl"]!, ForgotPasswordUrl = _loginOptions.Value.ForgotPasswordUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginModel model)
    {
        _pagePropertiesProvider.PreviousPagePath = _loginOptions.Value.BackUrl;
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        RpsAuthenticationTypes authenticationType = await  _loginService.AuthenticateAsync(model.Email.Value, model.Password);

        if (authenticationType == RpsAuthenticationTypes.Matched)
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, model.Email.Value) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Redirect(model.ReturnUrl);
        }

        if (authenticationType == RpsAuthenticationTypes.Unknown)
        {
            ModelState.AddModelError("Email.Value", "The email address or password you entered is incorrect");
        }
        else if (authenticationType == RpsAuthenticationTypes.Locked)
        {
            ModelState.AddModelError("Email.Value", "Your account is locked");
        }
        else if (authenticationType == RpsAuthenticationTypes.Outage)
        {
            ModelState.AddModelError("Email.Value", "There is an account login outage");
        }
        
        return View(model);
    }
}