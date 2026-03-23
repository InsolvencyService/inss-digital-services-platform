using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services;

[ExcludeFromCodeCoverage]
public sealed class NoopSubmitSectionService : ISubmitSectionService
{
    public static readonly ISubmitSectionService Default = new NoopSubmitSectionService();

    private NoopSubmitSectionService()
    {
    }
    
    public Task SubmitAsync(SectionModel section, string userSessionId)
    {
        return Task.CompletedTask;
    }
}