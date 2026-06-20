using Inss.Common;
using Inss.Common.IPUpload;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
// ReSharper disable UnusedParameter.Local

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class MockSubmitIPUploadSectionClient : ISubmitIPUploadSectionClient
{
    public Task<Result<SubmitIPUploadResponse>> SubmitAsync(SubmitIPUploadRequest submitRequest)
    {
        Console.WriteLine("Calling submission service...");
        return Task.FromResult<Result<SubmitIPUploadResponse>>(new SubmitIPUploadResponse { Reference = "RH12XY34" });
    }
}