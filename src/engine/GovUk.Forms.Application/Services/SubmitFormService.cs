using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services;

public sealed class SubmitFormService : ISubmitFormService
{
    public Task SubmitAsync(FormModel form, string userSessionId)
    {
        Console.WriteLine("Submitting the form to the integration layer...");
        return Task.CompletedTask;
    }
}