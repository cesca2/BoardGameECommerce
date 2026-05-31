public record CreateLoginRequest
{
    public string Email { get; init; }
    public string Password { get; init; }
}
