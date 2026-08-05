using Inss.Platform.Component.Resolvers;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Platform.Fip;

public sealed class StartPageResolver : IStartPageResolver
{
    public IActionResult Resolve()
    {
        return new RedirectResult("/search");
    }
}