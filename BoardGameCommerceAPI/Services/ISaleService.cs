
public interface ISaleService
{
    public List<Sale> GetSalesByCustomerId(Guid id);
    public Sale? GetSaleById(Guid id);
    public Sale CreateSale(Sale sale);
}