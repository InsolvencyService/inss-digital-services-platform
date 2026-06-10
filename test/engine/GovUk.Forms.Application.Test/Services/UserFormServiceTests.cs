using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.Services;

public class UserFormServiceTests
{
    private readonly IUserSessionProvider _userSessionProvider;
    private readonly IFormStorageProvider _formStorageProvider;
    private readonly ISubmitFormService _submitFormService;
    private readonly IFormFactory _formFactory;
    private readonly IFlowchart _sectionFlowchart;
    private readonly ServiceCollection _services = [];
    private readonly FormModel _form;
    private UserFormService _userFormService;
    private const string UserId = "UserId";
    
    public UserFormServiceTests()
    {
        _form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel section = _form.Sections["Your Details"];

        _userSessionProvider = Substitute.For<IUserSessionProvider>();
        _userSessionProvider.ResolveAsync().Returns(UserId);

        _formStorageProvider = Substitute.For<IFormStorageProvider>();
        _formStorageProvider.GetAsync(_form.Path, UserId).Returns(_form);

        _sectionFlowchart = Substitute.For<IFlowchart>();
        _services.AddKeyedSingleton(section.Path, _sectionFlowchart);

        _formFactory = Substitute.For<IFormFactory>();
        _formFactory.Create().Returns(_form);
        
        _submitFormService = Substitute.For<ISubmitFormService>();
    }
    
    [Fact]
    public async Task FormNotExists_GetAsync_TransitionsStartPagesToFirstNode()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        FullNameModel fullName = _form.Sections["Your Details"].Pages.GetFirstOf<FullNameModel>();
        
        await _userFormService.GetAsync(_form.Path);
        
        _sectionFlowchart.Received(1).TransitionPageToStart(fullName);
    }
    
    [Fact]
    public async Task FormNotExists_GetAsync_AssignsUserSessionIdToForm()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        
        await _userFormService.GetAsync(_form.Path);
        
        Assert.Equal(UserId, _form.Id);
    }
    
    [Fact]
    public async Task FormHasNoId_SaveAsync_NeverCallsSaveFormProvider()
    {
        _form.Id = ContentId.Empty;
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        
        await _userFormService.SaveAsync(_form);

        await _formStorageProvider.Received(0).SaveAsync(UserId, _form);
    }
    
    [Fact]
    public async Task FormHasId_SaveAsync_CallsSavesFormWithProvider()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        
        await _userFormService.SaveAsync(_form);

        await _formStorageProvider.Received(1).SaveAsync(UserId, _form);
    }
    
    [Fact]
    public async Task ForForm_RemoveAsync_CallsRemoveFormWithProvider()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        
        await _userFormService.RemoveAsync(_form);

        await _formStorageProvider.Received(1).RemoveAsync(UserId, _form);
    }
    
    [Fact]
    public async Task ForForm_RemoveAsync_ResetsFormId()
    {
        _formStorageProvider.ExistsAsync(_form.Path, UserId).Returns(false);
        BuildForService();
        
        await _userFormService.RemoveAsync(_form);

        Assert.Equal(ContentId.Empty, _form.Id);
    }
    
    [Fact]
    public async Task ContentIsForm_SubmitAsync_CallsFormSubmissionService()
    {
        BuildForService();

        await _userFormService.SubmitAsync(_form);
        
        await _submitFormService.Received(1).SubmitAsync(Arg.Is<FormModel>(f => f.Path == _form.Path), UserId);
    }
    
    private void BuildForService()
    {
        IServiceProvider serviceProvider = _services.BuildServiceProvider();
        _userFormService = new UserFormService(
            _userSessionProvider, _formStorageProvider, _formFactory, _submitFormService, serviceProvider);
    }
}