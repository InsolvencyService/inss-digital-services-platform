using Inss.FormsSubmission.Service.Extensions;

namespace Inss.FormsSubmission.Service.IPUpload.Processing;

public sealed class QueuedDynamicsHostedService : BackgroundService
{
    private readonly IBackgroundDynamicsQueue _queue;
    private readonly ILogger<QueuedDynamicsHostedService> _logger;

    public QueuedDynamicsHostedService(IBackgroundDynamicsQueue queue, ILogger<QueuedDynamicsHostedService> logger)
    {
        _queue = queue;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Func<CancellationToken, Task> backgroundTask = await _queue.DequeueAsync(stoppingToken);
            
            try
            {
                await backgroundTask(stoppingToken);
            }
            catch (Exception error)
            {
                _logger.DynamicsBackgroundTaskFailed(error.ToString());
            }
        }
    }
}