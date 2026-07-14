using System.Net;
using System.Net.Http.Headers;
using Inss.GovUk.Forms.IPUpload.Options;
using Microsoft.Identity.Client;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Handlers;

internal sealed class DynamicsAuthDelegatingHandler : DelegatingHandler
{
    private readonly IConfidentialClientApplication _clientApplication;
    private readonly string _scope;

    public DynamicsAuthDelegatingHandler(DynamicsOptions options) : base(new HttpClientHandler())
    {
        _clientApplication = ConfidentialClientApplicationBuilder
            .Create(options.ClientId)
            .WithClientSecret(options.ClientSecret)
            .WithAuthority($"https://login.microsoftonline.com/{options.TenantId}/")
            .Build();
        _scope = $"{options.Url}/.default";
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AuthenticationResult? authenticationResult = 
            await _clientApplication.AcquireTokenForClient([_scope]).ExecuteAsync(cancellationToken);

        if (authenticationResult is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Unable to get a Dynamics access token.")
        };
    }
}