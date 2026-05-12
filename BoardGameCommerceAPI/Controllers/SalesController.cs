using Microsoft.AspNetCore.Mvc;


namespace CommerceAPI.Controllers
{
    // specifies routing pattern for the controller [controller] is replaced with name of controller minus Controller suffix
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController(ISaleService saleService) : ControllerBase
    {
        private readonly ISaleService _saleService = saleService;


        [HttpPost]
        public ActionResult<Sale> CreateSale(Sale sale)
        {
            var newsale = _saleService.CreateSale(sale);
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

        [HttpDelete("{id}")]
        public ActionResult<Sale> DeleteSale(Guid id)
        {
            try
            {
                var result = _saleService.DeleteSale(id);
                if (result == null)
                {
                    return NotFound($"Could not find record with ID {id}");
                }

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });

            }

        }

    }}