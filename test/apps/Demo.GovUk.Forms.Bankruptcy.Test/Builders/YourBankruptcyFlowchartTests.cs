using Demo.GovUk.Forms.Bankruptcy.Application.Factories;
using Demo.GovUk.Forms.Bankruptcy.Builders;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Demo.GovUk.Forms.Bankruptcy.Test.Builders;

public class BankruptcyFlowchartTests
{
    [Fact]
    public void RegisteredFormWithBankruptcySection_Construct_RegistersFlowchartForSection()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<IServiceProvider>());
        BankruptcyFormFactory formFactory = new();
        FormModel form = formFactory.Create();
        SectionModel section = form.Sections["Bankruptcy"];
        builder.Services.AddSingleton<IFormFactory>(formFactory);
        YourBankruptcyFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}