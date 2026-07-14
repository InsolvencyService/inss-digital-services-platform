using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain.FormValidation;

internal sealed class SectionHasUniqueGroupsFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        List<string> errors = [];
        
        foreach (SectionModel section in form.Sections)
        {
            GroupPageModel[] groups = section.Pages.Where(p => p is GroupPageModel).Cast<GroupPageModel>().ToArray();

            List<GroupId> groupIds = [];

            foreach (GroupPageModel group in groups)
            {
                if (group.MetaData.Group == GroupId.Empty)
                {
                    errors.Add($"Missing group Id defined in section {section.Path}.");
                }
                else
                {
                    if (groupIds.Contains(group.MetaData.Group))
                    {
                        errors.Add($"Group Id {group.MetaData.Group} already defined in section {section.Path}.");
                    }
                    
                    groupIds.Add(group.MetaData.Group);
                }
            }
        }
        
        return errors.ToArray();
    }
}