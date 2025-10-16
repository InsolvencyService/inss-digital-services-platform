using INSS.Platform.Auth.API.Dto;
using System.Collections.Concurrent;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Provides an in-memory cache for storing and retrieving <see cref="RequestState"/> objects with expiration support.
    /// </summary>
    public class StateCache : IStateCache
    {
        private readonly ConcurrentDictionary<string, (RequestState State, DateTimeOffset Expiry)> _cache = new();
        private readonly ILogger<StateCache> _logger;
        private readonly TimeSpan _cacheItemExpiry;
        private readonly IConfiguration _appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCache"/> class.
        /// </summary>
        /// <param name="logger">The logger used for logging cache operations.</param>
        /// <param name="appConfig">The application configuration for cache settings.</param>
        public StateCache(ILogger<StateCache> logger, IConfiguration appConfig)
        {
            _logger = logger;
            _appConfig = appConfig;

            string? itemExpiryInMinutesStr = _appConfig["StateCache:ItemExpiryInMinutes"];
            if (double.TryParse(itemExpiryInMinutesStr, out double itemExpiryInMinutes))
            {
                _cacheItemExpiry = TimeSpan.FromMinutes(itemExpiryInMinutes);
            }
            else
            {
                _cacheItemExpiry = TimeSpan.FromMinutes(10);
            }
        }

        /// <inheritdoc/>
        public void Store(string stateKey, RequestState value)
        {
            DateTimeOffset expiry = DateTimeOffset.UtcNow.Add(_cacheItemExpiry);
            _cache[stateKey] = (value, expiry);
            _logger.LogInformation("Stored state for key: {StateKey} (expires at {Expiry}). Cache contains {CacheCount} items.", stateKey, expiry, _cache.Count);
        }

        /// <inheritdoc/>
        public bool TryGet(string stateKey, out RequestState value)
        {
            value = new RequestState();
            if (_cache.TryGetValue(stateKey, out (RequestState State, DateTimeOffset _) entry))
            {
                _cache.TryRemove(stateKey, out _);
                value = entry.State;
                _logger.LogInformation("State for key: {StateKey} retrieved and removed from cache. Cache contains {CacheCount} items.", stateKey, _cache.Count);
                return true;
            }
            else
            {
                _logger.LogError("State for key: {StateKey} not found in cache.", stateKey);
            }

            return false;
        }

        /// <inheritdoc/>
        public void CleanupExpired()
        {
            foreach (string key in _cache.Keys)
            {
                if (_cache.TryGetValue(key, out (RequestState State, DateTimeOffset Expiry) entry) && DateTimeOffset.UtcNow > entry.Expiry)
                {
                    _cache.TryRemove(key, out _);
                }
            }
        }
    }
}
