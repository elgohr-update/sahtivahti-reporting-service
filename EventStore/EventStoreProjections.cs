using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.Common.Log;
using System.Net;
using EventStore.ClientAPI.SystemData;

namespace ReportingService.EventStore
{
    public class EventStoreProjections : IEventStoreProjections
    {
        private readonly ILogger<EventStoreProjections> _logger;
        private readonly EventStoreConfig _config;

        public EventStoreProjections(ILogger<EventStoreProjections> logger, IOptions<EventStoreConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task<T> GetStateAsync<T>(string projectionName)
        {
            var manager = CreateProjectionsManager();

            var credentials = new UserCredentials(_config.Username, _config.Password);

            var result = await manager.GetStateAsync(projectionName, credentials);

            return JsonConvert.DeserializeObject<T>(result);
        }

        private ProjectionsManager CreateProjectionsManager()
        {
            return new ProjectionsManager(
                new ConsoleLogger(),
                new DnsEndPoint(_config.Host, 2113),
                TimeSpan.FromSeconds(10)
            );
        }
    }
}
