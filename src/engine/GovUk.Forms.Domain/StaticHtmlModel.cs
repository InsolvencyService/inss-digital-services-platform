namespace GovUk.Forms.Domain;

public class StaticHtmlModel : PageModel
{
    public string Key { get; set; }
    
    public string Html { get; set; }

    public override void CopyTo(PageModel target)
    {
        StaticHtmlModel staticHtml = target.As<StaticHtmlModel>();
        staticHtml.Key = Key;
        staticHtml.Html = Html;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Key = string.Empty;
        Html = string.Empty;
    }
}