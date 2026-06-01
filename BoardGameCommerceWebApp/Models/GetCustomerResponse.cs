public record GetCustomerResponse
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required Guid Id { get; init; }
}
