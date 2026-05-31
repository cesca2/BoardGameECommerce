using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceAPI.Controllers
{
    // specifies routing pattern for the controller [controller] is replaced with name of controller minus Controller suffix
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController(ISaleService saleService) : ControllerBase
    {
        private readonly ISaleService _saleService = saleService;

        [Authorize] //set role to admin
        [HttpGet("{id}")]
        public ActionResult<Sale> GetSaleById(Guid id)
        {
            try
            {
                var result = _saleService.GetSaleById(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet()]
        public ActionResult<List<Sale>> GetSales()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var result = _saleService.GetSalesByCustomerId(Guid.Parse(userId));

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Sale> CreateSale(SaleDTO sale)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine("USERID:" + userId);
            var salerequest = new Sale
            {
                Customer_Id = userId,
                Date = sale.Date,
                Time = sale.Time,
                QuantitiesByProductID = sale.QuantitiesByProductID,
            };

            var newsale = _saleService.CreateSale(salerequest);
            try
            {
                //return CreatedAtAction("Update me", new { id = sale.Id }, newsale);
                return Ok(newsale);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
