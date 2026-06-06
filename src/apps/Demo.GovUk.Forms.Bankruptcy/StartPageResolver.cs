using GovUk.Forms.Components.Resolvers;
using Microsoft.AspNetCore.Mvc;

namespace Demo.GovUk.Forms.Bankruptcy;

public sealed class StartPageResolver : IStartPageResolver
{
    public IActionResult Resolve()
    {
        WebRoot root = new();
        return new RedirectResult(root.Root);
    }
}