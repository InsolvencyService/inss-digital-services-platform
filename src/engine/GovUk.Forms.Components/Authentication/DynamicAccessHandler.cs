using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Components.Authentication;

public class DynamicAccessHandler : AuthorizationHandler<DynamicAccessRequirement>
{
    private readonly IAuthenticationProvider _authenticationProvider;

    public DynamicAccessHandler(IAuthenticationProvider authenticationProvider)
    {
        _authenticationProvider = authenticationProvider;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicAccessRequirement requirement)
    {
        if (context.Resource is DefaultHttpContext httpContext && _authenticationProvider.CanAccess(httpContext))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}