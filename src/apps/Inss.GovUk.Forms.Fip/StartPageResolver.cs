using GovUk.Forms.Components.Resolvers;
using Microsoft.AspNetCore.Mvc;

namespace Inss.GovUk.Forms.Fip;

public sealed class StartPageResolver : IStartPageResolver
{
    public IActionResult Resolve()
    {
        return new RedirectResult(WebInfo.Root);
    }
}