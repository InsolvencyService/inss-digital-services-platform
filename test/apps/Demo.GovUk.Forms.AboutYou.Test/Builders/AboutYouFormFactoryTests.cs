using Demo.GovUk.Forms.AboutYou.Application.Factories;
using Demo.GovUk.Forms.AboutYou.Builders;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Builders;

public class AboutYouFormFactoryTests
{
    [Fact]
    public void RegisteredFormWithYourDetailsSection_Construct_RegistersFlowchartForSection()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<IServiceProvider>());
        AboutYouFormFactory formFactory = new();
        FormModel form = formFactory.Create();
        SectionModel section = form.Sections["Your Details"];
        builder.Services.AddSingleton<IFormFactory>(formFactory);
        YourDetailsFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}