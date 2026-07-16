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
    
    public override string Format(string?[] values)
    {
        if (values.Length != 1)
        {
            throw new InvalidOperationException("Oops");
        }
        
        return $"<a href='{_resultDetailPath}/?key={WebUtility.UrlEncode(values[0])}' class='govuk-link'>{values[0]}</a>";
    }
}