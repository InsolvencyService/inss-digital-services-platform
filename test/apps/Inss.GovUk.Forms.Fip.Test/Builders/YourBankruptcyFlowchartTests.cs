using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.Fip.Application.Factories;
using Inss.GovUk.Forms.Fip.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.Fip.Test.Builders;

public class BankruptcyFlowchartTests
{
    [Fact]
    public void RegisteredFormWithFipSection_Construct_RegistersFlowchartForSection()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<IServiceProvider>());
        FipFormFactory formFactory = new();
        FormModel form = formFactory.Create();
        SectionModel section = form.Sections["Find an Insolvency Practitioner"];
        builder.Services.AddSingleton<IFormFactory>(formFactory);
        FipFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}