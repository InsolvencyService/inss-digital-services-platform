namespace GovUk.Forms.Domain.MetaData;

public sealed record QuestionPageMetaData(string? Text = null, string? Key = null) : PageMetaData2("Question", Text, Key);