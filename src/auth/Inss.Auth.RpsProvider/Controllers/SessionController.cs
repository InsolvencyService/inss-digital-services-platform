using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.RpsProvider.Controllers;

public class SessionController : Controller
{
    [HttpGet("/connect/endsession")]
    public async Task<IActionResult> EndSession()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("You have been logged out.");
    }
}