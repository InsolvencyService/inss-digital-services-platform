namespace Inss.Platform.Component.Options;

public sealed class FooterOptions
{
    public FooterLink[] Links { get; init; } = [];
    
    public sealed class FooterLink
    {
        public string Label { get; init; }
    
        public string Link { get; init; }
    }
}