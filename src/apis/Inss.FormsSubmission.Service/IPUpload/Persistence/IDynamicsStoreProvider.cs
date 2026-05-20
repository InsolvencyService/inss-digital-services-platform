namespace Inss.FormsSubmission.Service.IPUpload.Persistence;

public interface IDynamicsStoreProvider
{
    Task StoreAsync(DynamicsSubmission submission);
    Task<DynamicsSubmission?> GetAsync(string id, string reference);
}