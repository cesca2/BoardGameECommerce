
public interface ISaleService
{
    //public List<Sale>? GetAllSales();
    //public Sale? GetSale(Guid id);
    public Sale CreateSale(Sale sale);
    public string? DeleteSale(Guid id);
}