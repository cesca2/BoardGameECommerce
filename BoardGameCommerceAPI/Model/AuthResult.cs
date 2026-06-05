public record AuthResult
{
    public required bool Success { get; set; }
    public string? Error { get; set; }
    public string? Token { get; set; }
}
