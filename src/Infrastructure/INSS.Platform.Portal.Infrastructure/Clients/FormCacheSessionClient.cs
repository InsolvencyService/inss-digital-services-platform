using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain.Abstract;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.Json;

namespace INSS.Platform.Portal.Infrastructure.Clients;

/// <inheritdoc />
public class FormCacheSessionClient : IFormCacheClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormCacheSessionClient"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public FormCacheSessionClient(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public async Task<TForm> GetFormFromCacheAsync<TForm>() where TForm : FormBase, new()
    {
        HttpContext httpContext = GetHttpContext();

        string? formData = httpContext.Session.GetString(GetFormCacheKey<TForm>());
        
        return string.IsNullOrEmpty(formData) ? new TForm() : JsonSerializer.Deserialize<TForm>(formData) ?? new TForm();
    }

    /// <inheritdoc />
    public async Task<TFormList> GetFormListFromCacheAsync<TFormList>() where TFormList : FormBase, new()
    {
        HttpContext httpContext = GetHttpContext();

        string? formData = httpContext.Session.GetString(GetFormCacheKey<TFormList>());
        if (string.IsNullOrEmpty(formData))
        {
            return new TFormList();
        }

        TFormList? formList = JsonSerializer.Deserialize<TFormList>(formData);
        return formList ?? new TFormList();
    }

    /// <inheritdoc />
    public async Task<int> GetCurrentFormListIndexAsync()
    {
        HttpContext httpContext = GetHttpContext();

        string? indexData = httpContext.Session.GetString("CurrentIndex");
        
        return int.TryParse(indexData, out int index) ? index : 0;
    }

    /// <inheritdoc />
    public async Task<bool> SaveFormToCacheAsync<TForm>(TForm form) where TForm : FormBase
    {
        HttpContext httpContext = GetHttpContext();

        httpContext.Session.SetString(GetFormCacheKey<TForm>(), JsonSerializer.Serialize(form));

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> SaveFormListToCacheAsync<TFormList>(TFormList list) where TFormList : FormBase
    {
        HttpContext httpContext = GetHttpContext();

        httpContext.Session.SetString(GetFormCacheKey<TFormList>(), JsonSerializer.Serialize(list));

        return true;
    }

    /// <inheritdoc />
    public async Task SetCurrentFormListIndexAsync(int currentIndex)
    {
        HttpContext httpContext = GetHttpContext();

        httpContext.Session.SetString("CurrentIndex", currentIndex.ToString(CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public async Task IncrementCurrentFormListIndexAsync()
    {
        int currentIndex = await GetCurrentFormListIndexAsync();
        await SetCurrentFormListIndexAsync(currentIndex + 1);
    }

    private static string GetFormCacheKey<TForm>() where TForm : FormBase
    {
        return typeof(TForm).FullName!;
    }

    private HttpContext GetHttpContext()
    {
        return _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext.");
    }
}