using System.Threading.Tasks;
using System;

namespace ReportingService.Mq
{
    public interface IMq
    {
        void Consume<T>(string routingKey, Func<T, bool> onReceive);
    }
}
