using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow;

public interface IFlowchart
{
    FlowNode[] Nodes { get; }
    void AddNode(FlowNode node);
    ValueTask<ContentPath> PreProcessAsync(FormModel form, SectionModel section, PageModel page, string? state);
    ValueTask<ValidationResult[]> ValidateAsync(FormModel form, SectionModel section, PageModel page);
    ValueTask<ContentPath> ProcessAsync(FormModel form, SectionModel section, PageModel page);
    void TransitionPageToStart(PageModel page);
}