public record GetSaleResponse
{
    public Guid customer_Id { get; set; }
    public Dictionary<string, int> quantitiesByProductID { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public Guid Id { get; init; }
}