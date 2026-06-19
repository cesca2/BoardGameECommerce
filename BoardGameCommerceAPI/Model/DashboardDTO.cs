public record DashboardDTO
{
    public required int NumSalesTotal { get; set; }
    public required int NumCustomersTotal { get; set; }
    public required List<SalesProduct> Bestsellers { get; set; }
}
