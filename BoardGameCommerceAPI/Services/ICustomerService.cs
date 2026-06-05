public interface ICustomerService
{
    public AuthResult Login(LoginCustomerDTO customer);
    public AuthResult Register(CreateCustomerDTO customer);
    public Customer? GetCustomerById(Guid id);
    public List<Customer>? GetAllCustomers();
}
