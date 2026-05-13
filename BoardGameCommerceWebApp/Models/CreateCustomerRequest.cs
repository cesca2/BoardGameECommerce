public record CreateCustomerRequest
{
    public string Name { get; init; }
    public string Email { get; init; }
}