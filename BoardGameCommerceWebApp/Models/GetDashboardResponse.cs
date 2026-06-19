public record GetDashboardResponse
{
    public required int NumSalesTotal { get; init; }
    public required int NumCustomersTotal { get; init; }
    public required List<SalesProduct> Bestsellers { get; init; }
}
