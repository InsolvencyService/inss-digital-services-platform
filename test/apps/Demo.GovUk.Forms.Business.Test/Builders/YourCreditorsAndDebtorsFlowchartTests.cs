using Demo.GovUk.Forms.Business.Application.Factories;
using Demo.GovUk.Forms.Business.Builders;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Demo.GovUk.Forms.Business.Test.Builders;

public class YourCreditorsAndDebtorsFlowchartTests
{
    [Fact]
    public void RegisteredFormWithCreditorsAndDebtorsSection_Construct_RegistersFlowchartForSection()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<IServiceProvider>());
        BusinessFormFactory formFactory = new();
        FormModel form = formFactory.Create();
        SectionModel section = form.Sections["Creditors and Debtors"];
        builder.Services.AddSingleton<IFormFactory>(formFactory);
        YourCreditorsAndDebtorsFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}