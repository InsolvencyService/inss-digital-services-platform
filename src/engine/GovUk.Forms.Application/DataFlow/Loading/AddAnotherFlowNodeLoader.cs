using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Attributes;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class AddAnotherFlowNodeLoader : IFlowNodeLoader
{
    private const int FirstWorkingPageNodeIdIndex = 0;
    
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        AddAnotherModel addAnother = context.Page.As<AddAnotherModel>();
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(addAnother.MetaData.Group);
            groupInfo.Remove.LinkedToNode = context.Nodes.First(n => n.PagePath == groupInfo.Remove.Path).Id; // TODO: Context helper
        
        if (addAnother.Items.Count == 0)
        {
            return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[FirstWorkingPageNodeIdIndex]);
        }
        
        List<AddAnotherModel.AddAnotherSummaryModel> summary = [];

        int setIndex = 0;
            
        for (int i = 0; i < addAnother.Items.Count; i += groupInfo.WorkingPages.Count)
        {
            summary.Add(new AddAnotherModel.AddAnotherSummaryModel
            {
                Value = GetDisplayText(addAnother.Items[i]),
                ChangeUrl = $"{groupInfo.CheckAnswers.Path}/?state={setIndex}",
                RemoveUrl = $"{groupInfo.Remove.Path}/?state={setIndex}"
            });

            setIndex++;
        }

        addAnother.SummaryInfo = summary.ToArray();
        addAnother.GroupLength = groupInfo.WorkingPages.Count;

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