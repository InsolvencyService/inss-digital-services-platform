using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.PageFlow;

public interface ITreeViewManager
{
    ContentPath TransitionToStart(SectionModel section);
    ValueTask<PageModel> LoadAsync(FormModel form, SectionModel section, ContentPath path, Dictionary<string, string?> queryParams);
    ValueTask<ValidationResult[]> ValidateAsync(FormModel form, SectionModel section, PageModel page);
    ValueTask<ContentPath> SaveAsync(FormModel form, SectionModel section, PageModel page);
}