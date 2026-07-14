using Microsoft.AspNetCore.Authorization;

namespace GovUk.Forms.Components.Authentication;

public class DynamicAuthorizeAttribute : AuthorizeAttribute
{
    public DynamicAuthorizeAttribute()
    {
        Policy = "DynamicAccessPolicy";
    }
}