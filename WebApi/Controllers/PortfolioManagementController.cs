using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    public interface IPortfolioManagementController
    {
        Task<ActionResult<IEnumerable<Holding>>> GetHoldings();
        Task<ActionResult<Holding>> GetHolding(string tickerExchange);
        Task<ActionResult<Holding>> AddHolding(Holding holding);
        Task<ActionResult> EditHolding(string tickerExchange, Holding holding);
        Task<ActionResult> DeleteHolding(string tickerExchange);
        DateTime? GetLastModifiedDate();
        Task<ActionResult> UpdateLastModifiedDate(DateTime dateTime);
    }

    public class PortfolioManagementController : ControllerBase, IPortfolioManagementController
    {
        private readonly PortfolioManagementContext _context;

        public PortfolioManagementController(PortfolioManagementContext context)
        {
            _context = context;
            InitializeCashInPortfolio();
        }

        private void InitializeCashInPortfolio()
        {
            _context.Holdings.Add(new Holding() { TickerExchange = "USD", Name = "Cash", Weight = 1 });
            _context.SaveChanges();
        }

        public async Task<ActionResult<IEnumerable<Holding>>> GetHoldings() => await _context.Holdings.ToListAsync();

        public async Task<ActionResult<Holding>> GetHolding(string tickerExchange) => await _context.Holdings.SingleAsync(_ => _.TickerExchange == tickerExchange);

        public async Task<ActionResult<Holding>> AddHolding(Holding holding)
        {
            var cashHolding = _context.Holdings.Single(_ => _.TickerExchange == "USD");
            if (cashHolding.Weight < holding.Weight)
                throw new HttpRequestException("Not enough cash", null, System.Net.HttpStatusCode.InternalServerError);

            if (_context.Holdings.Any(_ => _.TickerExchange == holding.TickerExchange))
                throw new HttpRequestException("Holding already exists in portfolio", null, System.Net.HttpStatusCode.InternalServerError);

            _context.Holdings.Add(new Holding() { TickerExchange = holding.TickerExchange, Name = holding.Name, Weight = holding.Weight });
            cashHolding.Weight -= holding.Weight;
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<ActionResult> EditHolding(string tickerExchange, Holding holding)
        {
            if (tickerExchange != holding.TickerExchange)
                return BadRequest();

            var oldHolding = await _context.Holdings.FindAsync(tickerExchange);
            if (oldHolding == null)
                return NotFound();

            var cashHolding = _context.Holdings.Single(_ => _.TickerExchange == "USD");
            if (cashHolding.Weight < (holding.Weight - oldHolding.Weight))
                throw new HttpRequestException("Not enough cash", null, System.Net.HttpStatusCode.InternalServerError);
            var cashToAdd = oldHolding.Weight - holding.Weight;

            oldHolding.Name = holding.Name;
            oldHolding.Weight = holding.Weight;
            cashHolding.Weight += cashToAdd;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<ActionResult> DeleteHolding(string tickerExchange)
        {
            var cashHolding = _context.Holdings.Single(_ => _.TickerExchange == "USD");

            var holding = await _context.Holdings.FindAsync(tickerExchange);
            if (holding == null)
                return NotFound();

            _context.Holdings.Remove(holding);
            cashHolding.Weight += holding.Weight;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public DateTime? GetLastModifiedDate()
        {
            return _context.LastModifiedDate;
        }

        public async Task<ActionResult> UpdateLastModifiedDate(DateTime dateTime)
        {
            _context.LastModifiedDate = dateTime;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
