public interface ISaleRepository
{
    public List<Sale>? GetAllSales();
    public Sale? GetSale(Guid id);
    public int CreateSale(Sale sale);
    public int DeleteSale(Guid id);
}
