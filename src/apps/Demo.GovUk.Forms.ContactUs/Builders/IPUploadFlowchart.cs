using Demo.GovUk.Forms.ContactUs.Domain;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.ContactUs.Builders;

public sealed class IPUploadFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId fullNameId = NodeId.New();
        NodeId fileUploadId = NodeId.New();
        NodeId checkDetailsId = NodeId.New();
        NodeId addAnotherId = NodeId.New();
        NodeId removeId = NodeId.New();
        NodeId summaryId = NodeId.New();
        WebRoot webRoot = new();
        
        FormModel form = GetForm(services, webRoot.Root);
        SectionModel section = form.Sections["IP Upload"];
            
        FileUploadModel fileUpload = section.Pages.GetFirstOf<FileUploadModel>();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        PostSubmitSuccessModel  postSubmitSuccess = section.Pages.GetFirstOf<PostSubmitSuccessModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(declarationId, declaration.Path, fileUploadId)
            .WithLoader<StaticHtmlFlowNodeLoader>()
            .WithExecutor<DeclarationFlowNodeExecutor>()
            .Next()
            .AddDecisionNode(fileUploadId, fileUpload.Path, fileUploadErrorId, summaryId)
            .WithLoader<FileUploadFlowNodeLoader>()
            .WithValidator<FileUploadFlowNodeValidator>()
            .WithExecutor<FileUploadFlowNodeExecutor>()
            .Next()
            .AddSpurNode(fileUploadErrorId, uploadErrors.Path, fileUploadId, fileUploadErrorDetailsId)
            .WithLoader<FileUploadErrorFlowNodeLoader>()
            .Next()
            .AddTransitionNode(fileUploadErrorDetailsId, errorDetails.Path, fileUploadErrorId)
            .WithLoader<FileUploadErrorDetailsFlowNodeLoader>()
            .Next()
            .AddTransitionNode(summaryId, summary.Path, postSubmitSuccessId)
            .WithLoader<SectionSummaryFlowNodeLoader>()
            .WithExecutor<SubmitSectionFlowNodeExecutor>()
            .Next()
            .AddEndNode(postSubmitSuccessId, postSubmitSuccess.Path)
            .BuildAndRegister();
    }
}