using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportingService.EventStore;
using ReportingService.Model.Projection;

namespace ReportingService.Controllers
{
    public class FunnyFactsController : Controller
    {
        private readonly IEventStoreProjections _projections;

        public FunnyFactsController(IEventStoreProjections projections) : base()
        {
            _projections = projections;
        }

        [HttpGet("/v1/funnies")]
        public async Task<ActionResult<FunnyFact>> GetTotalCountCreatedItemsAction()
        {
            return await _projections.GetStateAsync<FunnyFact>("funny_facts");
        }
    }
}
