using System.Security.Claims;
using Inss.Platform.RpsProvider.Application.Services;
using Inss.Platform.RpsProvider.Domain.Enums;
using Inss.Platform.RpsProvider.Models;
using Inss.Platform.RpsProvider.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Inss.Platform.RpsProvider.Controllers;

public class LoginController : Controller
{
    private readonly ILoginService _loginService;
    private readonly IOptions<LoginOptions> _loginOptions;

    public LoginController(ILoginService  loginService, IOptions<LoginOptions> loginOptions)
    {
        _loginService = loginService;
        _loginOptions = loginOptions;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["PreviousPage"] = _loginOptions.Value.BackUrl;
        return View(new LoginModel{ ReturnUrl = Request.Query["returnUrl"]!, ForgotPasswordUrl = _loginOptions.Value.ForgotPasswordUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginModel model)
    {
        ViewData["PreviousPage"] = _loginOptions.Value.BackUrl;
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        RpsAuthenticationTypes authenticationType = await  _loginService.AuthenticateAsync(model.Email, model.Password);

        if (authenticationType == RpsAuthenticationTypes.Matched)
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, model.Email) };
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