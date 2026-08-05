namespace Inss.Platform.Domain.Components.Searching;

[Flags]
public enum SearchResultType
{
    Key = 1,
    Hidden = 2,
    Display = 4
}