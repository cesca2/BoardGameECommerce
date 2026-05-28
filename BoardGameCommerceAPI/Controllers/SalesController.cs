using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CommerceAPI.Controllers
{
    // specifies routing pattern for the controller [controller] is replaced with name of controller minus Controller suffix
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController(ISaleService saleService) : ControllerBase
    {
        private readonly ISaleService _saleService = saleService;

            [HttpGet]
        public ActionResult<List<Sale>> GetAllSales()
        {
            try{return Ok(_saleService.GetAllSales());}
            catch(ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }}
        
        [HttpGet("{id}")]
        public ActionResult<Sale> GetSaleById(Guid id)
        {
        try{
            var result = _saleService.GetSale(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
        }
        catch(ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
        }}

        [Authorize]
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