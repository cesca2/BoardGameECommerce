
using Microsoft.AspNetCore.Identity.Data;

public interface ICustomerService
{
    public AuthResult Login(LoginCustomerDTO customer);
    public AuthResult Register(CreateCustomerDTO customer);
    public Sale CreateSale(Sale sale);

    public Customer GetCustomerById(Guid id); // should probably move to admin

}