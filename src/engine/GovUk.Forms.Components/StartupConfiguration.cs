using System.Security.Cryptography;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Components.Authentication;
using GovUk.Forms.Components.Binding;
using GovUk.Forms.Components.Controllers;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Handlers;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using GovUk.Forms.Infrastructure.Extensions;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

[assembly: HostingStartup(typeof(GovUk.Forms.Components.StartupConfiguration))]

namespace GovUk.Forms.Components;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddOptions<HeaderOptions>()
                .Bind(context.Configuration.GetSection("Header"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<FooterOptions>()
                .Bind(context.Configuration.GetSection("Footer"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            BrokerOptions brokerOptions = new();
            context.Configuration.GetSection("Broker").Bind(brokerOptions);
            
            services.AddSingleton<IAuthenticationProvider>(_ =>
            {
                return brokerOptions.IdentityProvider switch
                {
                    IdentityProviderTypes.Rps => new RpsAuthenticationProvider(),
                    IdentityProviderTypes.Entra => new EntraAuthenticationProvider(),
                    IdentityProviderTypes.OneLogin => new OneLoginAuthenticationProvider(),
                    _ => new AnonymousAuthenticationProvider()
                };
            });
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    ConfigureBaseOptions(options, brokerOptions);

                    ConfigureTokenValidation(options, brokerOptions);

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = HandleProviderRedirect,
                        OnAuthenticationFailed = HandleAuthenticationFailed,
                        OnRedirectToIdentityProviderForSignOut = HandleProviderRedirectForSignOut
                    };
                });
            
            services.AddSingleton<IAuthorizationHandler, DynamicAccessHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddAuthorizationBuilder()
                .AddPolicy("DynamicAccessPolicy", policy => policy.Requirements.Add(new DynamicAccessRequirement()));
            
            services.AddApplication();
            services.AddInfrastructure();
            
            IMvcBuilder mvcBuilder = services
                .AddControllersWithViews(o => o.ModelBinderProviders.Insert(0, new ContentModelBinderProvider()))
                .AddApplicationPart(typeof(FormController).Assembly);
            RemoveNonHostedDiscoveredParts(mvcBuilder);
            
            services.AddSingleton<ITypeNameResolver, TypeNameResolver>();
            services.AddHttpClient();
            services.AddGovUkFrontend();
        });
        
        builder.Configure(app =>
        {
            /*
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            */
            
            app.UseGovUkFrontend();
            
            app.UseExceptionHandler("/error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "error",
                        pattern: "/error",
                        defaults: new { controller = "Error", action = "Index" })
                    .WithStaticAssets();

                IServiceProvider serviceProvider = endpoints.ServiceProvider;
                
                IEnumerable<IWebRoot> webRoots = serviceProvider.GetServices<IWebRoot>();
            
                foreach (IWebRoot webRoot in webRoots)
                {
                    IFormProvider formProvider = serviceProvider.GetRequiredService<IFormProvider>();
                    FormModel form = formProvider.Create(webRoot.Root);

                    endpoints.MapControllerRoute(
                            name: $"{form.Path.Value}/edit",
                            pattern: form.Path.Value,
                            defaults: new { controller = "Form", action = "Edit" })
                        .WithStaticAssets();

                    foreach (PageModel page in form.GetAllPages())
                    {
                        endpoints.MapControllerRoute(
                                name: $"{page.Path.Value}/edit",
                                pattern: page.Path.Value,
                                defaults: new { controller = "Form", action = "Edit" })
                            .WithStaticAssets();
                    }
                }
            
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Start}/{action=Index}/{id?}")
                    .WithStaticAssets();
                
                endpoints.MapStaticAssets();
            });
        });
    }

    private static string[] GetHostedAssemblyNames()
    {
        string environmentName = Environment.GetEnvironmentVariable("DOTNET_HOSTINGSTARTUPASSEMBLIES")!;
        var hostedAssemblies = environmentName
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        hostedAssemblies.Add("GovUk.Forms.HostApp");
        hostedAssemblies.Add("GovUk.Frontend.AspNetCore");
        return hostedAssemblies.ToArray();
    }

    private static void RemoveNonHostedDiscoveredParts(IMvcBuilder mvcBuilder)
    {
        string[] hostedAssemblyNames = GetHostedAssemblyNames();
        ApplicationPartManager partManager = mvcBuilder.PartManager;
        ApplicationPart[] applicationPartsToRemove = partManager.ApplicationParts.Where(
            part => !hostedAssemblyNames.Contains(part.Name)).ToArray();
            
        foreach (var part in applicationPartsToRemove)
        {
            partManager.ApplicationParts.Remove(part);
        }
    }
    
    private static void ConfigureBaseOptions(OpenIdConnectOptions options, BrokerOptions brokerOptions)
    {
        options.Authority = brokerOptions.Authority;
        options.ClientId = brokerOptions.ClientId;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SignedOutCallbackPath = "/signout-callback-oidc";
        options.SignedOutRedirectUri = "/";
        options.SaveTokens = true;
        options.Scope.Clear();

        foreach (string scope in brokerOptions.Scopes)
        {
            options.Scope.Add(scope);
        }
    }
    
    private static void ConfigureTokenValidation(OpenIdConnectOptions options, BrokerOptions brokerOptions)
    {
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.ProtocolValidator.RequireNonce = false;
                    
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(brokerOptions.JwtPublicKey);
                    
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    }
    
    private static Task HandleAuthenticationFailed(AuthenticationFailedContext context)
    {
        ILogger? logger = context.HttpContext.RequestServices.GetService<ILogger>();
        logger?.AuthenticationFailed(context.Exception.Message);
        context.Response.Redirect("/error");
        return Task.CompletedTask;
    }

    private static Task HandleProviderRedirect(RedirectContext context)
    {
        IAuthenticationProvider authenticationProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>();
        context.ProtocolMessage.LoginHint = authenticationProvider.Name;
        return Task.CompletedTask;
    }
    
    private static Task HandleProviderRedirectForSignOut(RedirectContext context)
    {
        IOptions<BrokerOptions> brokerOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<BrokerOptions>>();
        context.ProtocolMessage.PostLogoutRedirectUri = brokerOptions.Value.LogoutRedirectUrl;
        return Task.CompletedTask;
    }
}