public class DashboardService : IDashboardService
{
    IProductRepository _productRepository;
    ISaleRepository _saleRepository;
    ICustomerRepository _customerRepository;

    public DashboardService(
        IProductRepository productRepository,
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository
    )
    {
        _productRepository = productRepository;
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
    }

    public List<SalesProduct> GetBestsellers(int limit)
    {
        return _saleRepository.GetSalesProductsCount(limit);
    }

    public int? GetTotalSales()
    {
        return _saleRepository.CountSales();
    }

    public int? GetTotalCustomers()
    {
        return _customerRepository.CountCustomers();
    }
}
