public record GetSaleResponse
{
    public Guid customer_Id { get; init; }
    public Dictionary<string, int> quantitiesByProductID { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly Time { get; init; }
    public Guid Id { get; init; }
}
