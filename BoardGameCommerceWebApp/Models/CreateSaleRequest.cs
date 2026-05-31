public record CreateSaleRequest
{
    public Dictionary<string, int> quantitiesByProductID { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly Time { get; init; }
}
