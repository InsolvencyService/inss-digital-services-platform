using GovUk.Forms.Components.Resolvers;
using Microsoft.AspNetCore.Mvc;

namespace Inss.GovUk.Forms.Fip;

public sealed class StartPageResolver : IStartPageResolver
{
    public IActionResult Resolve()
    {
        WebRoot root = new();
        return new RedirectResult(root.Root);
    }
}