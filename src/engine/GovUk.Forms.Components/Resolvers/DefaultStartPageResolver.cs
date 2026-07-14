using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.Components.Resolvers;

public sealed class DefaultStartPageResolver : IStartPageResolver
{
    public IActionResult Resolve()
    {
        // This will expect the app to provide a _Start.cshtml view in its shared folder
        // If you want to override this behaviour, implement the interface in your app and register it to redirect to where you want
        return new ViewResult();
    }
}