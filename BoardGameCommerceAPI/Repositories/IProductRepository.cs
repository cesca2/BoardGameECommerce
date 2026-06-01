public interface IProductRepository
{
    public List<Product> GetAllProducts();
    public Product? GetProductById(Guid id);
}
