using System.ComponentModel;

namespace GovUk.Forms.Domain.Enums;

public enum SectionStateTypes
{
    [Description("Not started")]
    NotStarted,
    
    [Description("In progress")]
    InProgress,
    
    [Description("Completed")]
    Completed
}