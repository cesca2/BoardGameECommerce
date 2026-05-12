
public interface ICustomerService
{
    public List<Customer>? GetAllCustomers();
    public Customer? GetCustomer(Guid id);
    public Customer CreateCustomer(Customer customer);
    public string? DeleteCustomer(Guid id);
    public string? UpdateCustomer(Guid id, Customer customer);
}