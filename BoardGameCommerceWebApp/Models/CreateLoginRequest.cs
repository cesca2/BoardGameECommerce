public record CreateLoginRequest
{
    public string Email { get; set; }
    public string Password { get; init; }

}