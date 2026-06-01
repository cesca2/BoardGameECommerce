public interface ICustomerRepository
{
    public List<Customer>? GetAllCustomers();
    public Customer? GetCustomerById(Guid id);
    public Customer? GetCustomerByEmail(string email);
    public Customer? GetAdminByEmail(string email);
    public int CreateCustomer(Customer customer);
    public int DeleteCustomer(Guid id);
    public int UpdateCustomer(Guid id, Customer customer);
}
