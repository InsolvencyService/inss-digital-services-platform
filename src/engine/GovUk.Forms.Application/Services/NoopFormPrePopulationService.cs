using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services;

[ExcludeFromCodeCoverage]
public sealed class NoopFormPrePopulationService : IFormPrePopulationService
{
    public static readonly IFormPrePopulationService Default = new NoopFormPrePopulationService();

    private NoopFormPrePopulationService()
    {
    }
    
    public Task PrePopulateAsync(FormModel form, string userSessionId)
    {
        return Task.CompletedTask;
    }
}