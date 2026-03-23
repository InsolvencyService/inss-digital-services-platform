using GovUk.Forms.Components;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.AboutYou;

public sealed class WebRoot : IWebRoot
{
    public ContentPath Root => "/about-you";

    public string Name => "About You";
}