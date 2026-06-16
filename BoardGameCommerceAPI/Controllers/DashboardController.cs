using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        [HttpGet()]
        public ActionResult<Sale> GetSummary(int bestsellerLimit = 10)
        {
            try
            {
                var sales_total = _dashboardService.GetTotalSales() ?? -1;
                var customers_total = _dashboardService.GetTotalCustomers() ?? -1;
                var bestsellers = _dashboardService.GetBestsellers(bestsellerLimit);

                var result = new DashboardDTO
                {
                    NumSalesTotal = sales_total,
                    NumCustomersTotal = customers_total,
                    Bestsellers = bestsellers,
                };

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    };
}
