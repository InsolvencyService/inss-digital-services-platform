using Demo.GovUk.Forms.ContactUs.Application.Factories;
using Demo.GovUk.Forms.ContactUs.Builders;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Demo.GovUk.Forms.ContactUs.Test.Builders;

public class ContactUsFormFactoryTests
{
    [Fact]
    public void RegisteredFormWithYourDetailsSection_Construct_RegistersFlowchartForSection()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<IServiceProvider>());
        ContactUsFormFactory formFactory = new();
        FormModel form = formFactory.Create();
        SectionModel section = form.Sections["Send Us Files"];
        builder.Services.AddSingleton<IFormFactory>(formFactory);
        ContactUsFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}