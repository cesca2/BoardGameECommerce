using System.Reflection.Metadata.Ecma335;

public interface IProductService
{
    public List<Product>? GetAllProducts();
    public Product? GetProduct(Guid id);
}