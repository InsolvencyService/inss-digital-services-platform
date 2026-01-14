using System.Text.Json;

namespace INSS.Platform.AlphaDemo.Web.Helpers;

public static class FormSessionHelper
{
    public static TForm? LoadFormFromSession<TForm>(HttpContext httpContext, string sessionKey) where TForm : class, new()
    {
        string? json = httpContext.Session.GetString(sessionKey);
        return string.IsNullOrEmpty(json) ? new TForm() : JsonSerializer.Deserialize<TForm>(json);
    }
}