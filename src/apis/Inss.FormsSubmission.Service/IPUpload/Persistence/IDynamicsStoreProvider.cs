namespace Inss.FormsSubmission.Service.IPUpload.Persistence;

public interface IDynamicsStoreProvider
{
    Task StoreAsync(DynamicsSubmission submission);
}