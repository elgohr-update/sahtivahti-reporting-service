using System.Threading.Tasks;
namespace ReportingService.EventStore
{
    public interface IEventStore
    {
        Task Publish<T>(string eventType, T eventData);
    }
}
