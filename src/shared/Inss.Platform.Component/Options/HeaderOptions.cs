namespace Inss.Platform.Component.Options;

public sealed class HeaderOptions
{
    public string HomeLink { get; init; }
    
    public HeaderService Service { get; init; }
    
    public sealed class HeaderService
    {
        public string Label { get; init; }
    
        public string Link { get; init; }
    }
}