using GovUk.Forms.Components;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.Bankruptcy;

public sealed class WebRoot : IWebRoot
{
    public ContentPath Root => "/bankruptcy";
    
    public string Name => "Bankruptcy";
}