using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow;

public class FlowchartTests
{
    private readonly ILogger<Flowchart> _logger = Substitute.For<ILogger<Flowchart>>();
    private readonly FormModel _form;
    private readonly SectionModel _yourDetails;
    private readonly FlowNode _fullNameNode;
    private readonly FlowNode _addressNode;
    private readonly FlowNode _ageNode;
    private readonly FlowNode _salaryNode;
    private readonly FlowNode _bankAccountNode;
    private readonly FlowNode _summaryNode;
    private Flowchart _flowchart;
    private const string? NoState = null;
    
    public FlowchartTests()
    {
        NodeId fullNameId = "FullName";
        NodeId addressId = "Address";
        NodeId ageId = "Age";
        NodeId salaryId = "Salary";
        NodeId bankAccountId = "BankAccount";
        NodeId summaryId = "Summary";
    
        _form = TestFormModels.CreateWithYourDetailsSection();
        _yourDetails = _form.Sections[0];
        _fullNameNode = new FlowNode { Id = fullNameId, PagePath = _yourDetails.Pages[0].Path, NextNodes = [addressId] };
        _addressNode = new FlowNode { Id = addressId, PagePath = _yourDetails.Pages[1].Path, NextNodes = [ageId] };
        _ageNode = new FlowNode { Id = ageId, PagePath = _yourDetails.Pages[2].Path, NextNodes = [salaryId, summaryId] };
        _salaryNode = new FlowNode { Id = salaryId, PagePath = _yourDetails.Pages[3].Path, NextNodes = [bankAccountId, summaryId] };
        _bankAccountNode = new FlowNode { Id = bankAccountId, PagePath = _yourDetails.Pages[4].Path, NextNodes = [summaryId] };
        _summaryNode = new FlowNode { Id = summaryId, PagePath = _yourDetails.Pages[5].Path };
    }

    [Fact]
    public void InvalidNodeId_AddNode_ThrowsException()
    {
        BuildTestFlowchart([]);
        Assert.Throws<ArgumentException>(() => _flowchart.AddNode(new FlowNode { Id = NodeId.Empty, PagePath = "/form/section/page" }));
    }
    
    [Fact]
    public void DuplicateNodeId_AddNode_ThrowsException()
    {
        BuildTestFlowchart([]);
        FlowchartException exception = Assert.Throws<FlowchartException>(() => _flowchart.AddNode(_addressNode));
        
        Assert.Equal($"The node Id {_addressNode.Id} already exists.", exception.Message);
    }

    [Fact]
    public async Task NoNextNodeToLoad_PreProcessAsync_PutsSectionInStartedMode()
    {
        BuildTestFlowchart([]);
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        fullName.LinkedToNode = _fullNameNode.Id;
        
        await _flowchart.PreProcessAsync(_form, _yourDetails, fullName, NoState);

        Assert.NotNull(_yourDetails.StartedDate);
    }
    
    [Fact]
    public async Task NoNextNodeToLoad_PreProcessAsync_ReturnsPagePath()
    {
        BuildTestFlowchart([]);
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        fullName.LinkedToNode = _fullNameNode.Id;
        
        ContentPath path = await _flowchart.PreProcessAsync(_form, _yourDetails, fullName, NoState);
        
        Assert.Equal(fullName.Path, path);
    }
    
    [Fact]
    public async Task HasNextNodeToLoad_PreProcessAsync_ReturnsNextPagePath()
    {
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = _yourDetails.Pages.GetFirstOf<AgeModel>();
        IFlowNodeLoader testLoader = Substitute.For<IFlowNodeLoader>();
        testLoader.LoadAsync(Arg.Is<LoadContext>(c => c.Page.Path == fullName.Path)).Returns(_ageNode.Id);
        ServiceCollection services = [];
        services.AddKeyedSingleton(_fullNameNode.Id, testLoader);
        BuildTestFlowchart(services);
        fullName.LinkedToNode = _fullNameNode.Id;
        
        ContentPath path = await _flowchart.PreProcessAsync(_form, _yourDetails, fullName, NoState);
        
        Assert.Equal(age.Path, path);
    }

    [Fact]
    public async Task PostedPageWithChange_ProcessAsync_UpdatesSourcePage()
    {
        BuildTestFlowchart([]);
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        fullName.LinkedToNode = _fullNameNode.Id;
        fullName.Value = string.Empty;
        FullNameModel copyOfFullName = (FullNameModel)fullName.Clone();
        copyOfFullName.Value = "Homer Simpson";
        
        await _flowchart.ProcessAsync(_form, _yourDetails, copyOfFullName);
        
        Assert.Equal(copyOfFullName.Value, fullName.Value);
    }
    
    [Fact]
    public async Task PostedPageWithChange_ProcessAsync_SetsNextPagePreviousUrl()
    {
        BuildTestFlowchart([]);
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        fullName.LinkedToNode = _fullNameNode.Id;
        fullName.Value = string.Empty;
        FullNameModel copyOfFullName = (FullNameModel)fullName.Clone();
        copyOfFullName.Value = "Homer Simpson";
        AddressModel address = _yourDetails.Pages.GetFirstOf<AddressModel>();
        address.PreviousPagePath = "/";
        
        await _flowchart.ProcessAsync(_form, _yourDetails, copyOfFullName);
        
        Assert.Equal(fullName.Path, address.PreviousPagePath);
    }
    
    [Fact]
    public async Task PostedPageWithChange_ProcessAsync_UpdatesSourcePageCompleted()
    {
        BuildTestFlowchart([]);
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        fullName.LinkedToNode = _fullNameNode.Id;
        fullName.Value = string.Empty;
        FullNameModel copyOfFullName = (FullNameModel)fullName.Clone();
        copyOfFullName.Value = "Homer Simpson";
        
        await _flowchart.ProcessAsync(_form, _yourDetails, copyOfFullName);

        Assert.NotNull(fullName.CompletedDate);
    }
    
    [Fact]
    public async Task PostedPageWithChangeAndReturnUrl_ProcessAsync_ReturnsPageReturnUrl()
    {
        BuildTestFlowchart([]);
        AgeModel age = _yourDetails.Pages.GetFirstOf<AgeModel>();
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        fullName.LinkedToNode = _fullNameNode.Id;
        fullName.ReturnUrl = age.Path;
        fullName.Value = string.Empty;
        fullName.LinkedToNextNode = _addressNode.Id;
        FullNameModel copyOfFullName = (FullNameModel)fullName.Clone();
        copyOfFullName.Value = "Homer Simpson";
        
        ContentPath path = await _flowchart.ProcessAsync(_form, _yourDetails, copyOfFullName);
        
        Assert.Equal(age.Path, path);
    }

    [Fact]
    public void UnknownPagePath_TransitionPageToStart_ThrowsException()
    {
        BuildTestFlowchart([]);
        FullNameModel fullName = new() { Path = "/form/section/page", LinkedToNode = NodeId.Empty };

        FlowchartException exception = Assert.Throws<FlowchartException>(() => _flowchart.TransitionPageToStart(fullName));
        
        Assert.Equal($"Unable to find a node Id for page with path {fullName.Path}", exception.Message);
    }
    
    [Fact]
    public void KnownPagePath_TransitionPageToStart_LinksPageWithNode()
    {
        BuildTestFlowchart([]);
        FullNameModel fullName = _yourDetails.Pages.GetFirstOf<FullNameModel>();
        fullName.LinkedToNode = NodeId.Empty;
        
        _flowchart.TransitionPageToStart(fullName);
        
        Assert.Equal(_fullNameNode.Id, fullName.LinkedToNode);
    }

    private void BuildTestFlowchart(ServiceCollection services)
    {
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        
        _flowchart = new Flowchart(serviceProvider, _logger);
        _flowchart.AddNode(_fullNameNode);
        _flowchart.AddNode(_addressNode);
        _flowchart.AddNode(_ageNode);
        _flowchart.AddNode(_salaryNode);
        _flowchart.AddNode(_bankAccountNode);
        _flowchart.AddNode(_summaryNode);
    }
    
    [Fact]
    public async Task InvalidBankAccount_ValidateAsync_ReturnsValidationErrors()
    {
        ServiceCollection services = [];
        services.AddKeyedSingleton<IFlowNodeValidator, TestBankAccountFlowNodeValidator>(_bankAccountNode.Id);
        BuildTestFlowchart(services);
        BankAccountModel bankAccount = _yourDetails.Pages.GetFirstOf<BankAccountModel>();
        bankAccount.LinkedToNode = _bankAccountNode.Id;
        bankAccount.AccountNumber = "12345678";
        bankAccount.SortCode = "11-22-33";
        
        ValidationResult[] results = await _flowchart.ValidateAsync(bankAccount);
        
        AssertValidationError(results, nameof(bankAccount.AccountNumber), "The bank account details are invalid");
        AssertValidationError(results, nameof(bankAccount.SortCode), "The bank account details are invalid");
    }
    
    [Fact]
    public async Task ValidBankAccount_ValidateAsync_ReturnsNoValidationErrors()
    {
        ServiceCollection services = [];
        services.AddKeyedSingleton<IFlowNodeValidator, TestBankAccountFlowNodeValidator>(_bankAccountNode.Id);
        BuildTestFlowchart(services);
        TestSectionDefaults.YourDetails(_yourDetails);
        BankAccountModel bankAccount = _yourDetails.Pages.GetFirstOf<BankAccountModel>();
        bankAccount.LinkedToNode = _bankAccountNode.Id;
        bankAccount.AccountName = "H J Simpson";
        bankAccount.AccountNumber = "11223344";
        bankAccount.SortCode = "11-22-33";

        ValidationResult[] results = await _flowchart.ValidateAsync(bankAccount);
        
        Assert.Empty(results);
    }
    
    private static void AssertValidationError(ValidationResult[] results, string property, string message)
    {
        Assert.NotNull(results.FirstOrDefault(r => r.MemberNames.Contains(property)));
        Assert.NotNull(results.FirstOrDefault(r => r.ErrorMessage == message));
    }
    
    private sealed class TestBankAccountFlowNodeValidator : IFlowNodeValidator
    {
        public async ValueTask<ValidationResult[]> ValidateAsync(ValidateContext context)
        {
            ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
            List<ValidationResult> validationResults = baseValidationResults.ToList();
            
            BankAccountModel bankAccount = context.Page.As<BankAccountModel>();

            if (bankAccount is { AccountNumber: "12345678", SortCode: "11-22-33" })
            {
                validationResults.AddResult("The bank account details are invalid", ["AccountNumber", "SortCode"]);
            }

            return await ValueTask.FromResult(validationResults.ToArray());
        }
    }
}