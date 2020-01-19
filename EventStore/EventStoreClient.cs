using System.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace ReportingService.EventStore
{
    public class EventStoreClient : IEventStore
    {
        private readonly ILogger<EventStoreClient> _logger;
        private readonly EventStoreConfig _config;

        private static IEventStoreConnection _connection;

        public EventStoreClient(ILogger<EventStoreClient> logger, IOptions<EventStoreConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task Publish<T>(string eventType, T eventData)
        {
            if (_connection == null)
            {
                var connString = _config.ConnectionString;
                _connection = EventStoreConnection.Create(new Uri(connString));

                await _connection.ConnectAsync();
            }

            var jsonContent = JsonConvert.SerializeObject(eventData);

            var data = new List<EventData>
            {
                new EventData(
                    Guid.NewGuid(),
                    eventType,
                    true,
                    Encoding.UTF8.GetBytes(jsonContent),
                    new byte[]{}
                )
            };

            await _connection.AppendToStreamAsync("recipe-events", ExpectedVersion.Any, data);
        }
    }
}
