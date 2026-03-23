using GovUk.Forms.Components;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.IPUpload;

public sealed class WebRoot : IWebRoot
{
    public ContentPath Root => "/ip-upload";
    
    public string Name => "IP Upload";
}