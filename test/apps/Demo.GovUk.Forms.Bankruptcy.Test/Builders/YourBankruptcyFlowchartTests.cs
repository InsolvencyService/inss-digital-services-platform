using Demo.GovUk.Forms.Bankruptcy.Builders;
using Demo.GovUk.Forms.Bankruptcy.Factories;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
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
        IFormProvider formProvider = Substitute.For<IFormProvider>();
        formProvider.Create(form.Path).Returns(form);
        builder.Services.AddSingleton(formProvider);
        YourBankruptcyFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}