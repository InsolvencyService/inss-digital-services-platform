using System.Net;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain.Search.Formatting;

public sealed class ResultDetailLinkFieldValueFormatter : FieldValueFormatter
{
    private readonly ContentPath _resultDetailPath;

    public ResultDetailLinkFieldValueFormatter(ContentPath resultDetailPath)
    {
        _resultDetailPath = resultDetailPath;
    }
    
    public override string Format(string? value)
    {
        return $"<a href='{_resultDetailPath}/?key={WebUtility.UrlEncode(value)}' class='govuk-link'>{value}</a>";
    }
}