using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.GovUk.Forms.IPUpload.Builders;

public sealed class IPUploadFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId declarationId = "Declaration";
        NodeId caseRefNumId = "CaseReferenceNumber";
        NodeId employerId = "EmployerDetails";
        NodeId fileUploadId = "FileUpload";
        NodeId fileUploadErrorId = "FileUploadErrors";
        NodeId fileUploadErrorDetailsId = "FileUploadErrorDetails";
        NodeId summaryId = "Summary";
        NodeId postSubmitSuccessId = "PostSubmit";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["IP Upload"];
            
        IPUploadDeclarationModel declaration = section.Pages.GetFirstOf<IPUploadDeclarationModel>();
        CheckCaseReferenceModel checkCaseReference = section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        EmployerDetailsModel employerDetails = section.Pages.GetFirstOf<EmployerDetailsModel>();
        XmlFileUploadModel fileUpload = section.Pages.GetFirstOf<XmlFileUploadModel>();
        IPUploadXmlErrorsModel uploadErrors = section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        IPUploadXmlErrorDetailsModel errorDetails = section.Pages.GetFirstOf<IPUploadXmlErrorDetailsModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        PostSubmitModel  postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(declarationId, declaration.Path, caseRefNumId)
            .WithExecutor<DeclarationFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(caseRefNumId, checkCaseReference.Path, employerId)
            .WithValidator<CaseReferenceFlowNodeValidator>()
            .WithExecutor<CaseReferenceFlowNodeExecutor>()
            .Next()
            .AddDecisionNode(employerId, employerDetails.Path, caseRefNumId, fileUploadId)
            .WithExecutor<EmployerDetailsFlowNodeExecutor>()
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
            .WithLoader<IPUploadSummaryFlowNodeLoader>()
            .WithExecutor<SubmitFileUploadFlowNodeExecutor>()
            .Next()
            .AddEndNode(postSubmitSuccessId, postSubmit.Path, declarationId)
            .WithLoader<PostSubmitFlowNodeLoader>()
            .WithExecutor<PostSubmitFlowNodeExecutor>()
            .BuildAndRegister();
    }
}