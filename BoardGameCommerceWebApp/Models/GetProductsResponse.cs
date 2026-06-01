public record GetProductsResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int YearPublished { get; init; }
    public required float Price { get; init; }
}
