public interface ISaleRepository
{
    public List<Sale>? GetAllSales();
    public Sale? GetSale(Guid id);
    public int CreateSale(Sale sale);
    public int DeleteSale(Guid id);
    public int? CountSales();
    public List<SalesProduct>? GetSalesProductsCount(int limit = -1);
}
