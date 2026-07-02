namespace GovUk.Forms.Domain.MetaData;

public sealed record HintPageMetaData(string? Text = null, string? Key = null) : PageMetaData2("Hint", Text, Key);