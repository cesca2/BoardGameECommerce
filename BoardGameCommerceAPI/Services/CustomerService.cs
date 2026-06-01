using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordHasher<Customer> _hasher;
    private readonly IConfiguration _config;

    public CustomerService(
        ICustomerRepository customerRepository,
        IConfiguration config,
        IPasswordHasher<Customer> hasher
    )
    {
        _customerRepository = customerRepository;
        _config = config;
        _hasher = hasher;
    }

    public AuthResult Login(LoginCustomerDTO dto)
    {
        Customer? user;
        string role;
        // try for highest priveledge first
        user = _customerRepository.GetAdminByEmail(dto.Email);
        if (user is null)
        {
            user = _customerRepository.GetCustomerByEmail(dto.Email);
            role = "Customer";
            if (user is null)
            {
                return AuthResultFactory.Fail("Invalid login details provided");
            }
        }
        else
        {
            role = "Admin";
        }

        var hash = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (hash == PasswordVerificationResult.Success)
        {
            var token = GenerateJWT(user, role);
            return AuthResultFactory.Ok(token);
        }
        else
        {
            return AuthResultFactory.Fail("Invalid login details provided");
        }
    }

    // DANGER - only let customers not admins register with this method
    public AuthResult Register(CreateCustomerDTO dto)
    {
        var customerexists = _customerRepository.GetCustomerByEmail(dto.Email);

        if (customerexists is not null)
        {
            return AuthResultFactory.Fail("User with email already exists");
        }
        var customer = new Customer { Email = dto.Email, Name = dto.Name };

        customer.PasswordHash = _hasher.HashPassword(customer, dto.Password);

        var affected = _customerRepository.CreateCustomer(customer);

        if (affected == 0)
        {
            return AuthResultFactory.Fail("Customer was not created successfully");
        }

        var token = GenerateJWT(customer, "Customer");

        return AuthResultFactory.Ok(token);
    }

    public Customer? GetCustomerById(Guid id)
    {
        return _customerRepository.GetCustomerById(id);
    }

    // JWT token creation
    private string GenerateJWT(Customer customer, string role)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!)
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.NameIdentifier, customer.Name),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
