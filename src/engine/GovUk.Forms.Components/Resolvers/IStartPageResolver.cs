using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.Components.Resolvers;

public interface IStartPageResolver
{
    IActionResult Resolve();
}