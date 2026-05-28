
using Microsoft.AspNetCore.Identity.Data;

public interface ICustomerService
{
    public AuthResult Login(CustomerDTO customer);
    public AuthResult Register(CustomerDTO customer);
    public Sale CreateSale(Sale sale);

    public Customer GetCustomerById(Guid id); // should probably move to admin

}