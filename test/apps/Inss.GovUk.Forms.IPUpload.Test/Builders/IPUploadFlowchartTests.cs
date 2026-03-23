using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Builders;
using Inss.GovUk.Forms.IPUpload.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Builders;

public class IPUploadFlowchartTests
{
    [Fact]
    public void RegisteredFormWithIPUploadSection_Construct_RegistersFlowchartForSection()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<IServiceProvider>());
        IPUploadFormFactory formFactory = new();
        FormModel form = formFactory.Create();
        SectionModel section = form.Sections["IP Upload"];
        IFormProvider formProvider = Substitute.For<IFormProvider>();
        formProvider.Create(form.Path).Returns(form);
        builder.Services.AddSingleton(formProvider);
        builder.Services.AddSingleton(Substitute.For<IStaticContentProvider>());
        IPUploadFlowchart flowchart = new();
        
        flowchart.Construct(builder.Services);

        IFlowchart? registeredFlowchart = builder.Services.BuildServiceProvider().GetKeyedService<IFlowchart>(section.Path);
        Assert.NotNull(registeredFlowchart);
    }
}