using System.ComponentModel;

namespace GovUk.Forms.Domain.Enums;

[Flags]
public enum PageEditTypes
{
    [Description("Not started")]
    NotStarted = 0,
    
    [Description("Editing")]
    Editing = 1,
    
    [Description("Saved")]
    Saved = 2,
    
    [Description("Locked")]
    Locked = 4
}