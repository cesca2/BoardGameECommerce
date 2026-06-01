public record Customer
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public Guid Id { get; set; }
}
