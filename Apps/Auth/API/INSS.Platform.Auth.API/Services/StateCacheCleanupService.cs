namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Background service that periodically cleans up expired state objects from the cache.
    /// </summary>
    public class StateCacheCleanupService : BackgroundService
    {
        private readonly IStateCache _stateCache;
        private readonly IConfiguration _appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCacheCleanupService"/> class.
        /// </summary>
        /// <param name="stateCache">The state cache to clean up.</param>
        /// <param name="appConfig">The application configuration.</param>
        public StateCacheCleanupService(IStateCache stateCache, IConfiguration appConfig)
        {
            _stateCache = stateCache;
            _appConfig = appConfig;
        }

        /// <summary>
        /// Executes the background cleanup process, removing expired state objects at a configured interval.
        /// </summary>
        /// <param name="stoppingToken">A token that can be used to signal cancellation of the background operation.</param>
        /// <returns>A <see cref="Task"/> representing the background operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _stateCache.CleanupExpired();

                string? autoCleanupInMinutesStr = _appConfig["StateCache:AutoCleanupInMinutes"];
                if (double.TryParse(autoCleanupInMinutesStr, out double autoCleanupInMinutes))
                {
                    await Task.Delay(TimeSpan.FromMinutes(autoCleanupInMinutes), stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }
    }
}