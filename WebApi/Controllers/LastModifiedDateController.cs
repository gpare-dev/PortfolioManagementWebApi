using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LastModifiedDateController
    {
        private readonly IPortfolioManagementController _portfolioManagementController;

        public LastModifiedDateController(IPortfolioManagementController portfolioManagementController)
        {
            _portfolioManagementController = portfolioManagementController;
        }

        // GET: api/LastModifiedDate
        [HttpGet]
        public DateTime? GetLastModifiedDate() => _portfolioManagementController.GetLastModifiedDate();
    }
}
