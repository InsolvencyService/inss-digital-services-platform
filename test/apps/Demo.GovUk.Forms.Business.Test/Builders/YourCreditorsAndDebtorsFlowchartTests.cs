using Demo.GovUk.Forms.Business.Builders;
using Demo.GovUk.Forms.Business.Factories;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
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
        IFormProvider formProvider = Substitute.For<IFormProvider>();
        formProvider.Create(form.Path).Returns(form);
        builder.Services.AddSingleton(formProvider);
        YourCreditorsAndDebtorsFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}