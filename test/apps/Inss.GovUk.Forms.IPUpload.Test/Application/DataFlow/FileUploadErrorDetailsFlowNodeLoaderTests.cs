using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadErrorDetailsFlowNodeLoaderTests
{
    private readonly FileUploadErrorDetailsFlowNodeLoader _loader;
    
    public FileUploadErrorDetailsFlowNodeLoaderTests()
    {
        _loader = new FileUploadErrorDetailsFlowNodeLoader(Substitute.For<IPagePropertiesProvider>());
    }
    
    [Fact]
    public async Task EmptyQueryParams_LoadAsync_ThrowsException()
    {
        FlowNodeContext context = new() { QueryParams = [] };

        IPUploadException exception = await Assert.ThrowsAsync<IPUploadException>(() => _loader.LoadAsync(context).AsTask());
        
        Assert.Equal("Unable to load the IP upload error details as the error Id is unset.", exception.Message);
    }
}