
public interface IProductService
{
    public List<Product>? GetAllProducts();
    public Product? GetProductById(Guid id);
}