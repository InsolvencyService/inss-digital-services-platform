using INSS.Platform.Cache.Application.Repositories;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain.Abstract;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net;

namespace INSS.Platform.Portal.Infrastructure.Clients;

/// <inheritdoc />
public class FormCacheCosmosClient : IFormCacheClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheRepository _cacheRepository;

    public FormCacheCosmosClient(IHttpContextAccessor httpContextAccessor, ICacheRepository cacheRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _cacheRepository = cacheRepository;
    }

    /// <inheritdoc />
    public async Task<TForm> GetFormFromCacheAsync<TForm>() where TForm : FormBase, new()
    {
        Guid formId = GetFormId<TForm>();

        if (formId == Guid.Empty)
        {
            return new TForm();
        }

        TForm? formFromCache = await _cacheRepository.GetFormAsync<TForm>(formId);

        return formFromCache ?? new TForm();
    }

    /// <inheritdoc />
    public async Task<TFormList> GetFormListFromCacheAsync<TFormList>() where TFormList : FormBase, new()
    {
        Guid formId = GetFormId<TFormList>();

        if (formId == Guid.Empty)
        {
            return new TFormList();
        }

        TFormList? formFromCache = await _cacheRepository.GetFormAsync<TFormList>(formId);

        return formFromCache ?? new TFormList();
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
        SetFormIdentifiers(form);

        HttpStatusCode result = await _cacheRepository.SaveFormAsync(form);

        return result == HttpStatusCode.OK;
    }

    /// <inheritdoc />
    public async Task<bool> SaveFormListToCacheAsync<TFormList>(TFormList list) where TFormList : FormBase
    {
        SetFormIdentifiers(list);

        HttpStatusCode result = await _cacheRepository.SaveFormAsync(list);

        return result == HttpStatusCode.OK;
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

    private Guid GetFormId<TForm>() where TForm : FormBase
    {
        HttpContext httpContext = GetHttpContext();

        string? formId = httpContext.Session.GetString(GetFormCacheKey<TForm>());

        return string.IsNullOrEmpty(formId) ? Guid.Empty : Guid.Parse(formId);
    }

    private static string GetFormCacheKey<TForm>() where TForm : FormBase
    {
        return typeof(TForm).FullName!;
    }

    private HttpContext GetHttpContext()
    {
        return _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext.");
    }

    private void SetFormIdentifiers<TForm>(TForm form) where TForm : FormBase
    {
        SetFormSetId(form);
        SetFormId(form);
    }

    private void SetFormId<TForm>(TForm form) where TForm : FormBase
    {
        HttpContext httpContext = GetHttpContext();

        httpContext.Session.SetString(GetFormCacheKey<TForm>(), form.Id.ToString());
    }

    private void SetFormSetId(FormBase form)
    {
        const string formSetIdKey = "FormSetId";
        HttpContext httpContext = GetHttpContext();

        string? formSetIdStr = httpContext.Session.GetString(formSetIdKey);

        if (string.IsNullOrEmpty(formSetIdStr))
        {
            httpContext.Session.SetString(formSetIdKey, form.FormSetId.ToString());
        }
        else
        {
            if (Guid.TryParse(formSetIdStr, out Guid formSetId))
            {
                form.FormSetId = formSetId;
            }
        }
    }
}
