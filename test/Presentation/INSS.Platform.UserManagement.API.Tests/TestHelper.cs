using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace INSS.Platform.UserManagement.API.Tests;

internal static class TestHelper
{
    internal static ControllerContext CreateControllerContext(string? userName = null)
    {
        DefaultHttpContext httpContext = new();

        if (!string.IsNullOrEmpty(userName))
        {
            GenericIdentity identity = new(userName);
            httpContext.User = new ClaimsPrincipal(identity);
        }

        return new ControllerContext { HttpContext = httpContext };
    }
}
