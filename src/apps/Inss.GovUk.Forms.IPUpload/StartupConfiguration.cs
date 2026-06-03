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
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

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

            RpsApiOptions rpsOptions = context.Configuration.GetSection("Rps").Get<RpsApiOptions>()!;
            ExternalApiOptions submissionOptions = context.Configuration.GetSection("Submission").Get<ExternalApiOptions>()!;

            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddHttpClient<ICaseReferenceClient, MockCaseReferenceClient>(client =>
                    {
                        client.BaseAddress = new Uri(rpsOptions.Url);
                    });

                services.AddHttpClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(client =>
                    {
                        client.BaseAddress = new Uri(submissionOptions.Url);
                    });
            }
            else
            {
                services.AddHttpClient<ICaseReferenceClient, MockCaseReferenceClient>(client =>
                    {
                        client.BaseAddress = new Uri(rpsOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(rpsOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, rpsOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp,
                        rpsOptions.CountBeforeBreaking, rpsOptions.BreakDurationSeconds));

                services.AddHttpClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(client =>
                    {
                        client.BaseAddress = new Uri(submissionOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(submissionOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, submissionOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp,
                        submissionOptions.CountBeforeBreaking, submissionOptions.BreakDurationSeconds));

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
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, rpsOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp, 
                        rpsOptions.CountBeforeBreaking, rpsOptions.BreakDurationSeconds));
                */
            }

            services.AddTransient<ISubmitUploadedXmlService, SubmitUploadedXmlService>();

            IPUploadFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}