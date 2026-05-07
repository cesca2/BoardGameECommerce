using Microsoft.AspNetCore.Mvc;


namespace CommerceAPI.Controllers
{
    // specifies routing pattern for the controller [controller] is replaced with name of controller minus Controller suffix
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(ICommerceService commerceService) : ControllerBase
    {
        private readonly ICommerceService _commerceService = commerceService;

        [HttpGet]
        public ActionResult<List<Product>> GetAllProducts()
        {
            try{return Ok(_commerceService.GetAllProducts());}
            catch(ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }}
        
    }}