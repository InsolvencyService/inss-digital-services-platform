using System.Net;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Inss.FormsSubmission.Service.Options;

namespace Inss.FormsSubmission.Service.IPUpload;

internal sealed class DynamicsAuthDelegatingHandler : DelegatingHandler
{
    private readonly IConfidentialClientApplication _clientApplication;
    private readonly string _scope;

    public DynamicsAuthDelegatingHandler(DynamicsOptions options)
    {
        _clientApplication = ConfidentialClientApplicationBuilder
            .Create(options.ClientId)
            .WithClientSecret(options.ClientSecret)
            .WithAuthority($"{options.Url}/api/data/")
            .Build();
        _scope = $"{options.Url}/api/data/.default";
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