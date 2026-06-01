public interface ISaleService
{
    public List<Sale> GetSalesByCustomerId(Guid id);
    public Sale? GetSaleById(Guid id);
    public List<Sale> GetAllSales();
    public Sale? CreateSale(Sale sale);
}
