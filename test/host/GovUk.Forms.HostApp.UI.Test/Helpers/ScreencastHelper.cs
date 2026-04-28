
namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public sealed class ScreencastHelper
{
    private readonly IPage _page;
    private readonly string _filePath;
    private bool _started;

    public ScreencastHelper(IPage page, string filePath)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task StartAsync(AnnotatePosition annotatePosition = AnnotatePosition.TopRight)
    {
        if (_started)
        {
            return;
        }

        await _page.Screencast.StartAsync(new()
        {
            Path = _filePath
        });

        await _page.Screencast.ShowActionsAsync(new()
        {
            Position = annotatePosition
        });

        _started = true;
    }

    public async Task ShowChapterAsync(string title, string? description = null)
    {
        if (!_started)
        {
            return;
        }

        await _page.Screencast.ShowChapterAsync(title, new()
        {
            Description = description
        });
    }

    public async Task StopAsync()
    {
        if (!_started)
        {
            return;
        }

        await _page.Screencast.StopAsync();
        _started = false;
    }
}
