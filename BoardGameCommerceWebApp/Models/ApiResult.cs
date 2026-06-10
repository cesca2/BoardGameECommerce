public record ApiResult<T>
{
    public required bool Success { get; set; }
    public string? Error { get; set; }
    public T? Response { get; set; }
}
