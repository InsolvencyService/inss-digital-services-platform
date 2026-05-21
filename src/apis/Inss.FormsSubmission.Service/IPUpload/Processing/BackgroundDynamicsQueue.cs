using System.Threading.Channels;

namespace Inss.FormsSubmission.Service.IPUpload.Processing;

public sealed class BackgroundDynamicsQueue : IBackgroundDynamicsQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
    
    public async ValueTask QueueAsync(Func<CancellationToken, Task> backgroundTask)
    {
        await _queue.Writer.WriteAsync(backgroundTask);
    }

    public async ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}