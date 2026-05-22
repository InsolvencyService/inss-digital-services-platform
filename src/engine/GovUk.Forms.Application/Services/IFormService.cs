using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.Services;

public interface IFormService
{
    Task<(ContentModel? Content, ContentPath? RedirectTo)> LoadAsync(ContentPath requestPath, ContentPath refererPath, string? state);
    Task<ValidationResult[]> ValidateAsync(ContentModel postedContent);
    Task<ContentPath> SaveAsync(ContentModel postedContent);
}