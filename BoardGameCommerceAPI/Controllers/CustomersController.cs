using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CommerceAPI.Controllers
{
    // specifies routing pattern for the controller [controller] is replaced with name of controller minus Controller suffix
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService customerService) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;



        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<Customer> RegisterCustomer(CreateCustomerDTO customer)
        {

            try
            {
                var result = _customerService.Register(customer);
                if (result.Success == true)
                {
                    return StatusCode(201, new
                    {
                        token = result.Token
                    }
                );
                }
                else
                {
                    return BadRequest(result.Error);
                }
                ;
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<Customer> LoginCustomer(LoginCustomerDTO customer)
        {
            try
            {
                var result = _customerService.Login(customer);
                if (result.Success == true)
                {
                    return StatusCode(200, new
                    {
                        token = result.Token
                    }
                );
                }
                else
                {
                    return Unauthorized(result.Error);
                }
                ;
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }
        [Authorize]
        [HttpGet("me")]
        public ActionResult<CustomerDetailsDTO> Me()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = _customerService.GetCustomerById(Guid.Parse(userId));
                if (result is not null)
                {
                    return Ok(new CustomerDetailsDTO { Name = result.Name, Id = result.Id, Email = result.Email })
                ;
                }
                else
                {
                    return NotFound();
                }
                ;
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }
        /*

       
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Product> GetCustomerById(Guid id)
        {
        try{
            var result = _customerService.GetCustomerById(id);

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

        [Authorize]
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
        */

        /*
[Authorize]
[HttpGet]
public ActionResult<List<Product>> GetAllCustomers()
{
    try{return Ok(_customerService.GetAllCustomers());}
    catch(ApplicationException ex)
    {
        return StatusCode(500, new { error = ex.Message });
    }}





[HttpGet("lookup/{email}")]
public ActionResult<Product> GetCustomerByEmail(string email)
{
try{
    var result = _customerService.GetCustomerByEmail(email);

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
*/
    }
}