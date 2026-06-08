using GovUk.Forms.Components;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.Fip;

public sealed class WebRoot : IWebRoot
{
    public ContentPath Root => "/fip";
    
    public string Name => "Find an Insolvency Practitioner";
}