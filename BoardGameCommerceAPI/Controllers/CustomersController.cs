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
    }
}