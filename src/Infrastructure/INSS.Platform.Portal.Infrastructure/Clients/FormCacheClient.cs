using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.Json;

namespace INSS.Platform.Portal.Infrastructure.Clients;

/// <summary>
/// Provides methods for caching and retrieving form data in the session.
/// </summary>
public class FormCacheClient : IFormCacheClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormCacheClient"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public FormCacheClient(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public TForm? GetFormFromCache<TForm>(string cacheKey) where TForm : FormBase, new()
    {
        HttpContext httpContext = GetHttpContext();

        string? formData = httpContext.Session.GetString(cacheKey);
        
        return string.IsNullOrEmpty(formData) ? new TForm() : JsonSerializer.Deserialize<TForm>(formData);
    }

    /// <inheritdoc />
    public List<TForm> GetFormListFromCache<TForm>(string cacheKey) where TForm : FormBase, new()
    {
        HttpContext httpContext = GetHttpContext();

        string? formData = httpContext.Session.GetString(cacheKey);

        if (string.IsNullOrEmpty(formData))
        {
            return new List<TForm>();
        }

        List<TForm>? formList = JsonSerializer.Deserialize<List<TForm>>(formData);
        return formList ?? new List<TForm>();
    }

    /// <inheritdoc />
    public int GetCurrentFormListIndex(string cacheKey)
    {
        HttpContext httpContext = GetHttpContext();

        string? indexData = httpContext.Session.GetString($"{cacheKey}_CurrentIndex");
        
        return int.TryParse(indexData, out int index) ? index : 0;
    }

    /// <inheritdoc />
    public TForm SetFormToCache<TForm>(string cacheKey, TForm form) where TForm : FormBase
    {
        HttpContext httpContext = GetHttpContext();

        httpContext.Session.SetString(cacheKey, JsonSerializer.Serialize(form));

        return form;
    }

    /// <inheritdoc />
    public List<TForm> SetFormListToCache<TForm>(string cacheKey, IEnumerable<TForm> forms) where TForm : FormBase
    {
        HttpContext httpContext = GetHttpContext();

        httpContext.Session.SetString(cacheKey, JsonSerializer.Serialize(forms));

        return forms is List<TForm> list ? list : [.. forms];
    }

    /// <inheritdoc />
    public void SetCurrentFormListIndex(string cacheKey, int currentIndex)
    {
        HttpContext httpContext = GetHttpContext();

        httpContext.Session.SetString($"{cacheKey}_CurrentIndex", currentIndex.ToString(CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public void IncrementCurrentFormListIndex(string cacheKey)
    {
        int currentIndex = GetCurrentFormListIndex(cacheKey);
        SetCurrentFormListIndex(cacheKey, currentIndex + 1);
    }

    /// <inheritdoc />
    public string GetFormCacheKey<TForm>(string cacheKeySuffix = "") where TForm : FormBase
    {
        return $"{typeof(TForm).FullName}{cacheKeySuffix}";
    }

    private HttpContext GetHttpContext()
    {
        return _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext.");
    }
}