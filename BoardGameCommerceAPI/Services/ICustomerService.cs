
public interface ICustomerService
{
    public List<Customer>? GetAllCustomers();
    public Customer? GetCustomerById(Guid id);
    public Customer? GetCustomerByEmail(string email);
    public Customer CreateCustomer(Customer customer);
    public string? DeleteCustomer(Guid id);
    public string? UpdateCustomer(Guid id, Customer customer);
}