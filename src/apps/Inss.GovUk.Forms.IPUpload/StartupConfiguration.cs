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
using System.Net.Http.Headers;
using System.Net.Mime;
using GovUk.Forms.Application.Factories;
using Inss.GovUk.Forms.IPUpload.Application.Factories;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Infrastructure.Handlers;

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
            services.AddSingleton<IFormFactory, IPUploadFormFactory>();
            
            services.AddTransient<ICaseReferenceService, CaseReferenceService>();

            DynamicsOptions dynamicsOptions = context.Configuration.GetSection("Dynamics").Get<DynamicsOptions>()!;
            ExternalApiOptions submissionOptions = context.Configuration.GetSection("Submission").Get<ExternalApiOptions>()!;

            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddHttpClient<ICaseReferenceClient, MockCaseReferenceClient>(client =>
                    {
                        client.BaseAddress = new Uri(dynamicsOptions.Url);
                    });

                services.AddHttpClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(client =>
                    {
                        client.BaseAddress = new Uri(submissionOptions.Url);
                    });
            }
            else
            {
                services.AddHttpClient<ICaseReferenceClient, CaseReferenceClient>(client =>
                    {
                        client.BaseAddress = new Uri($"{dynamicsOptions.Url}/");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new DynamicsAuthDelegatingHandler(dynamicsOptions))
                    .SetHandlerLifetime(TimeSpan.FromMinutes(dynamicsOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, dynamicsOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(
                        sp, dynamicsOptions.CountBeforeBreaking, dynamicsOptions.BreakDurationSeconds));

                services.AddHttpClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(client =>
                    {
                        client.BaseAddress = new Uri(submissionOptions.Url);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(submissionOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, submissionOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp,
                        submissionOptions.CountBeforeBreaking, submissionOptions.BreakDurationSeconds));
            }

            services.AddTransient<ISubmitUploadedXmlService, SubmitUploadedXmlService>();
            services.AddTransient<IValidationFactory, ValidationFactory>();
            
            IPUploadFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}