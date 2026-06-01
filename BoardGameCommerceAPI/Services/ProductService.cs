public class ProductService : IProductService
{
    IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public List<Product> GetAllProducts()
    {
        return _productRepository.GetAllProducts();
    }

    public Product? GetProductById(Guid id)
    {
        return _productRepository.GetProductById(id);
    }
}
