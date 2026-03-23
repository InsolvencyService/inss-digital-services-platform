using GovUk.Forms.Components;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.Business;

public sealed class WebRoot : IWebRoot
{
    public ContentPath Root => "/business";
    
    public string Name => "Business";
}