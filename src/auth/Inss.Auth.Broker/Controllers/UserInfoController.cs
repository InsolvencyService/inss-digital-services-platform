using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.Broker.Controllers;

public class UserInfoController : Controller
{
    [HttpGet("/connect/userinfo")]
    public IActionResult UserInfo()
    {
        // TODO: Need to workout why the user is not authenticated for this to work. The cookie should contain the identity!
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
        return Json(claims);
    }
}