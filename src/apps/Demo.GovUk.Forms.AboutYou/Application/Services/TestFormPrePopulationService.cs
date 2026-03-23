using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Application.Services;

public sealed class TestFormPrePopulationService : IFormPrePopulationService
{
    public Task PrePopulateAsync(FormModel form, string userSessionId)
    {
        Console.WriteLine($"Populating the form extension for {form.Path} and user {userSessionId}");
        return Task.CompletedTask;
    }
}