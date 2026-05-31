public record CreateCustomerRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; init; }
}
