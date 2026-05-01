using GovUk.Forms.Components;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.ContactUs;

public sealed class WebRoot : IWebRoot
{
    public ContentPath Root => "/contact-us";

    public string Name => "Contact Us";
}