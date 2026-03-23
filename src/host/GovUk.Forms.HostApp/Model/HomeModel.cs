using GovUk.Forms.Components;

namespace GovUk.Forms.HostApp.Model;

public sealed class HomeModel
{
    public FormModel[] Forms { get; init; } = [];

    public static HomeModel Create(IEnumerable<IWebRoot> webRoots)
    {
        return new HomeModel { Forms = webRoots.Select(wr => new FormModel{Root = wr.Root, Name = wr.Name}).ToArray() };
    }
}