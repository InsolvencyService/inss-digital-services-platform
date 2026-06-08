namespace Inss.FormsSubmission.Service.IPUpload.Persistence;

public interface IDynamicsStoreProvider
{
    Task StoreAsync(DynamicsSubmission submission, CancellationToken cancellationToken);
    Task<DynamicsSubmission?> GetAsync(string id, string reference, CancellationToken cancellationToken);
    Task<DynamicsSubmission[]> GetByReferenceAsync(string reference, CancellationToken cancellationToken);
}