using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadFlowNodeValidatorTests
{
    [Fact]
    public async Task UnsupportedFilenameExtension_ValidateAsync_ReturnsErrorDetails()
    {
        ILogger<FileUploadFlowNodeValidator> logger = Substitute.For<ILogger<FileUploadFlowNodeValidator>>();
        FileUploadFlowNodeValidator validator = new(logger);
        XmlFileUploadModel xmlFileUpload = new() { Filename = "upload.txt" };
        FlowNode node = new() { Id = "NodeId1", PagePath = xmlFileUpload.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = xmlFileUpload };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "The file must end with an XML extension", "Contents");
    }
    
    private static void AssertError(ValidationResult result, string message, string property)
    {
        Assert.Equal(message, result.ErrorMessage);
        Assert.NotNull(result.MemberNames.FirstOrDefault(p => p == property));
    }
}