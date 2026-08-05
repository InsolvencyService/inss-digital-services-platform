using Microsoft.AspNetCore.Mvc;

namespace Inss.Platform.Component.Resolvers;

public interface IStartPageResolver
{
    IActionResult Resolve();
}