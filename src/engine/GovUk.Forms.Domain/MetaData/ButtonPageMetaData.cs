namespace GovUk.Forms.Domain.MetaData;

public sealed record ButtonPageMetaData(string? Text = null, string? Key = null) : PageMetaData2("Button", Text, Key);