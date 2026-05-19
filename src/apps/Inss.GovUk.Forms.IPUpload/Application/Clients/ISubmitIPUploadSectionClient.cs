using Inss.Common;
using Inss.Common.IPUpload;

namespace Inss.GovUk.Forms.IPUpload.Application.Clients;

public interface ISubmitIPUploadSectionClient
{
    Task<Result<SubmitIPUploadResponse>> SubmitAsync(SubmitIPUploadRequest submitRequest);
}