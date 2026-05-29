public record GetCustomerResponse
{
    public string Name { get; init; }
    public string Email { get; init; }
    public Guid Id { get; init; }
}