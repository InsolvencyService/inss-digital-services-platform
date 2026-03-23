using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Attributes;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class AddAnotherRemoveFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        RemoveModel remove = context.Page.As<RemoveModel>();
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(remove.MetaData.Group);
        remove.SetIndex = int.Parse(context.State!, CultureInfo.CurrentCulture); // TODO: Sort bang!
        remove.ReturnUrl = groupInfo.AddAnother.Path;
        PageModel[] subItems = groupInfo.AddAnother.Items.Skip(remove.SetIndex * groupInfo.WorkingPages.Count).Take(groupInfo.WorkingPages.Count).ToArray();
        PageModel firstPageForRemoving = subItems[0];
        remove.RemoveQuestion = $"Do you want to remove {GetDisplayText(firstPageForRemoving)}?";

        return ValueTask.FromResult<NodeId?>(null);
    }
    
    private static string GetDisplayText(PageModel page)
    {
        PropertyInfo[] properties = page.GetType().GetProperties().Where(
            p => p.GetCustomAttribute<CopyableAttribute>() is not null).ToArray();
        PropertyInfo property = page.GetType().GetProperties().FirstOrDefault(
            p => p.GetCustomAttribute<SummaryAttribute>() is not null) ?? properties[0];
        object? value = property.GetValue(page, null);
        DisplayFormatAttribute? displayValueFormat = property.GetCustomAttribute<DisplayFormatAttribute>();
        string displayValue = displayValueFormat?.DataFormatString is not null
            ? string.Format(CultureInfo.CurrentCulture, displayValueFormat.DataFormatString, value)
            : value?.ToString() ?? string.Empty;
        return displayValue;
    }
}