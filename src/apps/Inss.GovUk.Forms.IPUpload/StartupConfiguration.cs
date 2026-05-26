using GovUk.Forms.Components;
using Inss.Common.Infrastructure;
using Inss.Common.Infrastructure.Options;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Builders;
using Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;
using Inss.GovUk.Forms.IPUpload.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(Inss.GovUk.Forms.IPUpload.StartupConfiguration))]

namespace Inss.GovUk.Forms.IPUpload;

[ExcludeFromCodeCoverage]
public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);

            services.AddTransient<ICaseReferenceService, CaseReferenceService>();

            ExternalApiOptions submissionOptions = context.Configuration.GetSection("Submission").Get<ExternalApiOptions>()!;

            // Enable below once we have the dynamics work complete

            /*if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddTypedClient<ISubmitIPUploadSectionClient, MockSubmitIPUploadSectionClient>(submissionOptions);
            }
            else
            {
                services.AddTypedClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(submissionOptions);
            }*/

            services.AddHttpClient<ISubmitIPUploadSectionClient, MockSubmitIPUploadSectionClient>(client =>
                {
                    client.BaseAddress = new Uri(submissionOptions.Url);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(submissionOptions.LifetimeMinutes))
                .AddPolicyHandler(Resilience.GetRetryPolicy(submissionOptions.RetryCount))
                .AddPolicyHandler(Resilience.GetCircuitBreaker(submissionOptions.CountBeforeBreaking, submissionOptions.BreakDurationSeconds));

            RpsApiOptions rpsOptions = context.Configuration.GetSection("Rps").Get<RpsApiOptions>()!;

            services.AddHttpClient<ICaseReferenceClient, MockCaseReferenceClient>(client =>
                {
                    client.BaseAddress = new Uri(rpsOptions.Url);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(rpsOptions.LifetimeMinutes))
                .AddPolicyHandler(Resilience.GetRetryPolicy(rpsOptions.RetryCount))
                .AddPolicyHandler(Resilience.GetCircuitBreaker(rpsOptions.CountBeforeBreaking, rpsOptions.BreakDurationSeconds));

            // Enable below once we have deployment of the listener in the RPS environment

            /*if (context.HostingEnvironment.IsDevelopment())
            
            RpsApiOptions rpsOptions = context.Configuration.GetSection("Rps").Get<RpsApiOptions>()!;
            ExternalApiOptions submissionOptions = context.Configuration.GetSection("Submission").Get<ExternalApiOptions>()!;
            
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddHttpClient<ICaseReferenceClient, MockCaseReferenceClient>(client =>
                    {
                        client.BaseAddress = new Uri(rpsOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(rpsOptions.LifetimeMinutes))
                    .AddPolicyHandler(Resilience.GetRetryPolicy(rpsOptions.RetryCount))
                    .AddPolicyHandler((Resilience.GetCircuitBreaker(
                        rpsOptions.CountBeforeBreaking, rpsOptions.BreakDurationSeconds)));
                
                services.AddHttpClient<ISubmitIPUploadSectionClient, MockSubmitIPUploadSectionClient>(client =>
                    {
                        client.BaseAddress = new Uri(submissionOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(submissionOptions.LifetimeMinutes))
                    .AddPolicyHandler(Resilience.GetRetryPolicy(submissionOptions.RetryCount))
                    .AddPolicyHandler((Resilience.GetCircuitBreaker(
                        submissionOptions.CountBeforeBreaking, submissionOptions.BreakDurationSeconds)));
            }
            else
            {
                services.AddHttpClient<ICaseReferenceClient, MockCaseReferenceClient>(client =>
                    {
                        client.BaseAddress = new Uri(rpsOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(rpsOptions.LifetimeMinutes))
                    .AddPolicyHandler(Resilience.GetRetryPolicy(rpsOptions.RetryCount))
                    .AddPolicyHandler((Resilience.GetCircuitBreaker(
                        rpsOptions.CountBeforeBreaking, rpsOptions.BreakDurationSeconds)));
                
                services.AddHttpClient<ISubmitIPUploadSectionClient, MockSubmitIPUploadSectionClient>(client =>
                    {
                        client.BaseAddress = new Uri(submissionOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(submissionOptions.LifetimeMinutes))
                    .AddPolicyHandler(Resilience.GetRetryPolicy(submissionOptions.RetryCount))
                    .AddPolicyHandler((Resilience.GetCircuitBreaker(
                        submissionOptions.CountBeforeBreaking, submissionOptions.BreakDurationSeconds)));
                
                // Disabled until we get the RPS listener in place
                /*
                services.AddOptions<RpsApiOptions>()
                    .Bind(context.Configuration.GetSection("Rps"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
                
                services.AddHttpClient<ICaseReferenceClient, CaseReferenceClient>(client =>
                    {
                        client.BaseAddress = new Uri(rpsOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(rpsOptions.LifetimeMinutes))
                    .AddPolicyHandler(Resilience.GetRetryPolicy(rpsOptions.RetryCount))
                    .AddPolicyHandler((Resilience.GetCircuitBreaker(
                        rpsOptions.CountBeforeBreaking, rpsOptions.BreakDurationSeconds)));
                
                services.AddHttpClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(client =>
                    {
                        client.BaseAddress = new Uri(submissionOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(submissionOptions.LifetimeMinutes))
                    .AddPolicyHandler(Resilience.GetRetryPolicy(submissionOptions.RetryCount))
                    .AddPolicyHandler((Resilience.GetCircuitBreaker(
                        submissionOptions.CountBeforeBreaking, submissionOptions.BreakDurationSeconds)));
                */
            }
            
            services.AddTransient<ISubmitUploadedXmlService, SubmitUploadedXmlService>();

            IPUploadFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}