using Microsoft.AspNetCore.Mvc;


namespace CommerceAPI.Controllers
{
    // specifies routing pattern for the controller [controller] is replaced with name of controller minus Controller suffix
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        public ActionResult<List<Product>> GetAllProducts([FromQuery] PaginationParams paginationParams)
        {

            try{
                var products = _productService.GetAllProducts();
                if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
                {
                    products = products.Where(x=>x.Name.ToLower().Contains(paginationParams.SearchTerm.ToLower())).ToList();
                }
                return Ok(products);
                }
            catch(ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }}
        
    

    [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(Guid id)
        {
        try{
            var result = _productService.GetProductById(id);

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
        
}}