using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Microsoft.Extensions.DependencyInjection;
using XmlFileUploadModel = Inss.GovUk.Forms.IPUpload.Domain.XmlFileUploadModel;

namespace Inss.GovUk.Forms.IPUpload.Builders;

public sealed class IPUploadFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId declarationId = NodeId.New();
        NodeId fileUploadId = NodeId.New();
        NodeId fileUploadErrorId = NodeId.New();
        NodeId summaryId = NodeId.New();
        NodeId postSubmitSuccessId = NodeId.New();
        WebRoot webRoot = new();
        
        FormModel form = GetForm(services, webRoot.Root);
        SectionModel section = form.Sections["IP Upload"];
            
        StaticHtmlModel declaration = section.Pages.GetFirstOf<StaticHtmlModel>();
        XmlFileUploadModel fileUpload = section.Pages.GetFirstOf<XmlFileUploadModel>();
        IPUploadXmlErrorsModel uploadErrors = section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
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
            .AddTransitionNode(fileUploadErrorId, uploadErrors.Path, fileUploadId)
            .WithLoader<FileUploadErrorFlowNodeLoader>()
            .Next()
            .AddTransitionNode(summaryId, summary.Path, postSubmitSuccessId)
            .WithLoader<SectionSummaryFlowNodeLoader>()
            .WithExecutor<SubmitSectionFlowNodeExecutor>()
            .Next()
            .AddEndNode(postSubmitSuccessId, postSubmitSuccess.Path)
            .BuildAndRegister();
    }
}