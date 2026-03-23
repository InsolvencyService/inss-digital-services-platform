using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Validating;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadFlowNodeValidatorTests
{
    [Fact]
    public async Task UnsupportedFilenameExtension_ValidateAsync_ReturnsErrorDetails()
    {
        FileUploadFlowNodeValidator validator = new();
        XmlFileUploadModel xmlFileUpload = new() { Filename = "upload.txt" };
        FlowNode node = new() { Id = "NodeId1", PagePath = xmlFileUpload.Path };
        ValidateContext context = new() { Nodes = [node], CurrentNode = node, Page = xmlFileUpload };
        
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