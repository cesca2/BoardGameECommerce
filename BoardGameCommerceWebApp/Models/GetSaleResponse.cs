public record GetSaleResponse
{
    public required Guid customer_Id { get; init; }
    public required Dictionary<string, int> quantitiesByProductID { get; init; }
    public required DateOnly Date { get; init; }
    public required TimeOnly Time { get; init; }
    public required Guid Id { get; init; }
}
