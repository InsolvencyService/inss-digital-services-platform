using Demo.GovUk.Forms.ContactUs.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.ContactUs.Builders;

public sealed class ContactUsFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId fullNameId = "FullName";
        NodeId fileUploadId = "FileUpload";
        NodeId addAnotherId = "AddAnother";
        NodeId removeId = "Remove";
        NodeId summaryId = "Summary";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Send Us Files"];

        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        FileUploadModel fileUpload = section.Pages.GetFirstOf<FileUploadModel>();
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(fullNameId, fullName.Path, fileUploadId)
            .Next()
            .AddTransitionNode(fileUploadId, fileUpload.Path, addAnotherId)
            .WithExecutor<AddAnotherWorkingPageFlowNodeExecutor>()
            .Next()
            .AddDecisionNode(addAnotherId, addAnother.Path, fileUploadId, summaryId)
            .WithLoader<AddAnotherUploadFilesFlowNodeLoader>()
            .WithExecutor<AddAnotherFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(removeId, remove.Path, addAnotherId)
            .WithLoader<AddAnotherRemoveFlowNodeLoader>()
            .WithExecutor<AddAnotherRemoveFlowNodeExecutor>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<ContactUsSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}