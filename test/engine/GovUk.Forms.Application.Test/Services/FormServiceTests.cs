using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.Services;

public class FormServiceTests
{
    private readonly IUserFormService _userFormService; 
    private readonly IFlowchart _sectionFlowchart;
    private readonly ServiceCollection _services = [];
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private FormService _formService;
    private const string? NoState = null;
    
    public FormServiceTests()
    {
        _form = TestFormModels.CreateWithYourDetailsSection();
        _section = _form.Sections["Your Details"];
        _userFormService = Substitute.For<IUserFormService>();
        _sectionFlowchart = Substitute.For<IFlowchart>();
        _services.AddKeyedSingleton(_section.Path, _sectionFlowchart);
    }
    
    [Fact]
    public async Task ContentIsFormWithSingleSection_LoadAsync_ReturnsRedirectToFirstPage()
    {
        _userFormService.GetAsync(_form.Path).Returns(_form);
        BuildForService();
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(_form.Path, NoState);
        
        Assert.Equal(fullName.Path, result.RedirectTo);
    }
    
    [Fact]
    public async Task ContentIsFormWithMultipleSections_LoadAsync_ReturnsRedirectToFirstPage()
    {
        _userFormService.GetAsync(_form.Path).Returns(_form);
        BuildForService();
        _form.Sections.Add(_section);
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(_form.Path, NoState);
        
        Assert.Equal(_form, result.Content);
    }
    
    [Fact]
    public async Task ContentIsPage_LoadAsync_ReturnsPage()
    {
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        _userFormService.GetAsync(fullName.Path).Returns(_form);
        BuildForService();
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(fullName.Path, NoState);
        
        Assert.Equal(fullName, result.Content);
    }
    
    [Fact]
    public async Task PageIsSummaryAndSectionComplete_LoadAsync_ReturnsSummaryPage()
    {
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        _userFormService.GetAsync(summary.Path).Returns(_form);
        BuildForService();
        _section.SetCompleted();
        
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(summary.Path, NoState);
        
        Assert.Equal(summary, result.Content);
    }
    
    [Fact]
    public async Task AnyContent_LoadAsync_SavesForm()
    {
        _userFormService.GetAsync(_form.Path).Returns(_form);
        BuildForService();
        
        await _formService.LoadAsync(_form.Path, NoState);

        await _userFormService.Received(1).SaveAsync(_form);
    }

    [Fact]
    public async Task ContentIsForm_ValidateAsync_ReturnsNoErrors()
    {
        _userFormService.GetAsync(_form.Path).Returns(_form);
        BuildForService();
        
        ValidationResult[] result = await _formService.ValidateAsync(_form);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task ContentIsValidPage_ValidateAsync_ReturnsValidationResults()
    {
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        _userFormService.GetAsync(fullName.Path).Returns(_form);
        BuildForService();
        _sectionFlowchart.ValidateAsync(fullName).Returns([]);
        
        ValidationResult[] result = await _formService.ValidateAsync(fullName);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task ContentIsInvalidPage_ValidateAsync_ReturnsValidationResults()
    {
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        _userFormService.GetAsync(fullName.Path).Returns(_form);
        BuildForService();
        _sectionFlowchart.ValidateAsync(fullName).Returns([new ValidationResult("Error")]);
        
        ValidationResult[] result = await _formService.ValidateAsync(fullName);
        
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task ContentIsPage_SaveAsync_ReturnsPath()
    {
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = _section.Pages.GetFirstOf<AgeModel>();
        _userFormService.GetAsync(fullName.Path).Returns(_form);
        BuildForService();
        _sectionFlowchart.ProcessAsync(_form, _section, fullName).Returns(age.Path);

        ContentPath result = await _formService.SaveAsync(fullName);
        
        Assert.Equal(age.Path, result);
    }
    
    [Fact]
    public async Task ContentIsPage_SaveAsync_SavesForm()
    {
        FullNameModel fullName = _section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = _section.Pages.GetFirstOf<AgeModel>();
        _userFormService.GetAsync(fullName.Path).Returns(_form);
        BuildForService();
        _sectionFlowchart.ProcessAsync(_form, _section, fullName).Returns(age.Path);

        await _formService.SaveAsync(fullName);
        
        await _userFormService.Received(1).SaveAsync(_form);
    }
    
    private void BuildForService()
    {
        IServiceProvider serviceProvider = _services.BuildServiceProvider();
        _formService = new FormService(_userFormService, serviceProvider);
    }
}