namespace Inss.Platform.Submission.Service.IPUpload.Processing;

public interface IBackgroundDynamicsQueue
{
    ValueTask QueueAsync(Func<CancellationToken, Task> backgroundTask);
    ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}