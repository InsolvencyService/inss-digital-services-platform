using Inss.Common;
using Inss.Common.IPUpload;
using Inss.Platform.Ipus.Application.Clients;

// ReSharper disable UnusedParameter.Local

namespace Inss.Platform.Ipus.Infrastructure.Clients;

public sealed class MockSubmitIPUploadSectionClient : ISubmitIPUploadSectionClient
{
    public Task<Result<SubmitIPUploadResponse>> SubmitAsync(SubmitIPUploadRequest submitRequest)
    {
        Console.WriteLine("Calling submission service...");
        return Task.FromResult<Result<SubmitIPUploadResponse>>(new SubmitIPUploadResponse { Reference = "RH12XY34" });
    }
}