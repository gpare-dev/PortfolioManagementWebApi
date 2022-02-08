using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Controllers;

namespace WebApi.Filters
{
    public class LogFilter : IActionFilter
    {
        private readonly IPortfolioManagementController _portfolioManagementController;

        public LogFilter(IPortfolioManagementController portfolioManagementController)
        {
            _portfolioManagementController = portfolioManagementController;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _portfolioManagementController.UpdateLastModifiedDate(DateTime.Now);
        }

        public void OnActionExecuting(ActionExecutingContext context) { }
    }
}
