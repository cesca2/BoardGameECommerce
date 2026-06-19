public interface IDashboardService
{
    public List<SalesProduct> GetBestsellers(int limit);
    public int? GetTotalSales();
    public int? GetTotalCustomers();
}
