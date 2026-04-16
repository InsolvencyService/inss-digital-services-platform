using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.Services;

public class FormServiceTests
{
    private readonly IUserSessionProvider _userSessionProvider;
    private readonly IFormStorageProvider _formStorageProvider;
    private readonly ISubmitFormService _submitFormService;
    private readonly IFormProvider _formProvider;
    private readonly IFlowchart _sectionFlowchart;
    private readonly ServiceCollection _services = [];
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private FormService _formService;
    private const string? NoState = null;
    private const string UserId = "UserId";
    
    public FormServiceTests()
    {
        _form = TestFormModels.CreateWithYourDetailsSection();
        _section = _form.Sections["Your Details"];
        
        _userSessionProvider = Substitute.For<IUserSessionProvider>();
        _userSessionProvider.ResolveAsync().Returns(UserId);

        _formStorageProvider = Substitute.For<IFormStorageProvider>();
        _formStorageProvider.GetAsync(_form.Path, UserId).Returns(_form);

        _sectionFlowchart = Substitute.For<IFlowchart>();
        _services.AddKeyedSingleton(_section.Path, _sectionFlowchart);

        _formProvider = Substitute.For<IFormProvider>();
        _formProvider.Create(_form.Path).Returns(_form);
        
        _submitFormService = Substitute.For<ISubmitFormService>();
    }
    
    [Fact]
    public async Task FormNotExists_LoadAsync_TransitionsStartPagesToFirstNode()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        FullNameModel fullName = _form.Sections["Your Details"].Pages.GetFirstOf<FullNameModel>();
        
        await _formService.LoadAsync(_form.Path, NoState);
        
        _sectionFlowchart.Received(1).TransitionPageToStart(fullName);
    }
    
    [Fact]
    public async Task FormNotExists_LoadAsync_AssignsUserSessionIdToForm()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        
        await _formService.LoadAsync(_form.Path, NoState);
        
        Assert.Equal(UserId, _form.Id);
    }
    
    [Fact]
    public async Task ContentIsFormWithSingleSection_LoadAsync_ReturnsRedirectToFirstPage()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(_form.Path, NoState);
        
        Assert.Equal(fullName.Path, result.RedirectTo);
    }
    
    [Fact]
    public async Task ContentIsFormWithMultipleSections_LoadAsync_ReturnsRedirectToFirstPage()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        _form.Sections.Add(_section);
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(_form.Path, NoState);
        
        Assert.Equal(_form, result.Content);
    }
    
    [Fact]
    public async Task ContentIsPage_LoadAsync_ReturnsPage()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(fullName.Path, NoState);
        
        Assert.Equal(fullName, result.Content);
    }
    
    [Fact]
    public async Task PageIsSummaryAndSectionComplete_LoadAsync_ReturnsSummaryPage()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        _section.State = SectionStateTypes.Completed;
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(summary.Path, NoState);
        
        Assert.Equal(summary, result.Content);
    }
    
    [Fact]
    public async Task AnyContent_LoadAsync_SavesForm()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        
        await _formService.LoadAsync(_form.Path, NoState);

        await _formStorageProvider.Received(1).SaveAsync(UserId, _form);
    }

    [Fact]
    public async Task ContentIsForm_ValidateAsync_ReturnsNoErrors()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        
        ValidationResult[] result = await _formService.ValidateAsync(_form);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task ContentIsValidPage_ValidateAsync_ReturnsValidationResults()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        _sectionFlowchart.ValidateAsync(fullName).Returns([]);
        
        ValidationResult[] result = await _formService.ValidateAsync(fullName);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task ContentIsInvalidPage_ValidateAsync_ReturnsValidationResults()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        _sectionFlowchart.ValidateAsync(fullName).Returns([new ValidationResult("Error")]);
        
        ValidationResult[] result = await _formService.ValidateAsync(fullName);
        
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ContentIsForm_SaveAsync_CallsFormSubmissionService()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();

        await _formService.SaveAsync(_form);
        
        await _submitFormService.Received(1).SubmitAsync(Arg.Is<FormModel>(f => f.Path == _form.Path), UserId);
    }
    
    [Fact]
    public async Task ContentIsPage_SaveAsync_ReturnsPath()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = _section.Pages.GetFirstOf<AgeModel>();
        _sectionFlowchart.ProcessAsync(_form, _section, fullName).Returns(age.Path);

        ContentPath result = await _formService.SaveAsync(fullName);
        
        Assert.Equal(age.Path, result);
    }
    
    [Fact]
    public async Task ContentIsPage_SaveAsync_SavesForm()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(true);
        BuildForService();
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = _section.Pages.GetFirstOf<AgeModel>();
        _sectionFlowchart.ProcessAsync(_form, _section, fullName).Returns(age.Path);

        await _formService.SaveAsync(fullName);
        
        await _formStorageProvider.Received(1).SaveAsync(UserId, _form);
    }
    
    private void BuildForService()
    {
        IServiceProvider serviceProvider = _services.BuildServiceProvider();
        _formService = new FormService(_formStorageProvider, _userSessionProvider, _submitFormService, _formProvider, serviceProvider);
    }
}