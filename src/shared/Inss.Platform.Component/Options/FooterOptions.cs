// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global - Options
// ReSharper disable UnusedAutoPropertyAccessor.Global - Options
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