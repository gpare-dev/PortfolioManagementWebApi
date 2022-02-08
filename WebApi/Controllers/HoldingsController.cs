using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoldingsController
    {
        private readonly IPortfolioManagementController _portfolioManagementController;

        public HoldingsController(IPortfolioManagementController portfolioManagementController)
        {
            _portfolioManagementController = portfolioManagementController;
        }

        // GET: api/Holdings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Holding>>> GetHoldings() => await _portfolioManagementController.GetHoldings();

        // GET: api/Holdings/TickerExchange
        [HttpGet("{tickerExchange}")]
        public async Task<ActionResult<Holding>> GetHolding(string tickerExchange) => await _portfolioManagementController.GetHolding(tickerExchange);

        // POST: api/Holdings
        [HttpPost]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ActionResult<Holding>> PostHolding(Holding holding) => await _portfolioManagementController.AddHolding(holding);

        // PUT: api/Holdings/TickerExchange
        [HttpPut("{tickerExchange}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<IActionResult> PutHolding(string tickerExchange, Holding holding) => await _portfolioManagementController.EditHolding(tickerExchange, holding);

        // DELETE: api/Holdings/TickerExchange
        [HttpDelete("{tickerExchange}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<IActionResult> DeleteHolding(string tickerExchange) => await _portfolioManagementController.DeleteHolding(tickerExchange);
    }
}
