using System.Security.Claims;
using Inss.Auth.RpsProvider.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.RpsProvider.Controllers;

public class LoginController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new LoginModel{ ReturnUrl = Request.Query["returnUrl"]! });
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginModel model)
    {
        // TODO: Implement integration with RPS
        if (ModelState.IsValid && 
            model.Email.Equals("test@user.org", StringComparison.OrdinalIgnoreCase) && 
            model.Password.Equals("test123",  StringComparison.OrdinalIgnoreCase))
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, model.Email) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Redirect(model.ReturnUrl);
        }
        
        if (ModelState.ErrorCount == 0)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
        }
        
        return View();
    }
}