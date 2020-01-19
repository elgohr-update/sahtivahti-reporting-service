using System.Threading.Tasks;

namespace ReportingService.EventStore
{
    public interface IEventStoreProjections
    {
        Task<T> GetStateAsync<T>(string projectionName);
    }
}
