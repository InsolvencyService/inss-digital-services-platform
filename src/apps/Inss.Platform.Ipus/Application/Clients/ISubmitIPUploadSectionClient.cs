using Inss.Common;
using Inss.Common.IPUpload;

namespace Inss.Platform.Ipus.Application.Clients;

public interface ISubmitIPUploadSectionClient
{
    Task<Result<SubmitIPUploadResponse>> SubmitAsync(SubmitIPUploadRequest submitRequest);
}