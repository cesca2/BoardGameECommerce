using Microsoft.AspNetCore.Mvc;


namespace CommerceAPI.Controllers
{
    // specifies routing pattern for the controller [controller] is replaced with name of controller minus Controller suffix
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService customerService) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;

        [HttpGet]
        public ActionResult<List<Product>> GetAllCustomers()
        {
            try{return Ok(_customerService.GetAllCustomers());}
            catch(ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }}
        
    

    [HttpGet("{id}")]
        public ActionResult<Product> GetCustomerById(Guid id)
        {
        try{
            var result = _customerService.GetCustomer(id);

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

                [HttpPost]
        public ActionResult<Customer> CreateCustomer(Customer customer)
        {
            var newcustomer = _customerService.CreateCustomer(customer);
            try
            {
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, newcustomer);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        public ActionResult<Customer> DeleteCustomer(Guid id)
        {
            try
            {
                var result = _customerService.DeleteCustomer(id);
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

        [HttpPut("{id}")]
        public ActionResult<Customer> UpdateCustomer(Guid id, [FromBody] Customer updatedCustomer)
        {
  
            try
            {

                if (id != updatedCustomer.Id)
                {
                    return BadRequest(new
                    {
                        errors = new[]
                        {
                        "Id is invalid, must match value in new Customer"
                    }
                    });
                }
                var result = _customerService.UpdateCustomer(id, updatedCustomer);

                if (result == null)
                {
                    return NotFound();
                }

                return Created("", result);
            }
            catch (ApplicationException ex)
            {
                
                return StatusCode(500, new { error = ex.Message });

            }
        }
}}