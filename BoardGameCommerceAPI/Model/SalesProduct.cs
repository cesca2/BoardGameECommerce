public record SalesProduct
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int SalesItemsTotal { get; set; }
}
