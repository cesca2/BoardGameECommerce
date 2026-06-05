using System.ComponentModel.DataAnnotations;

public record CreateCustomerDTO
{
    [MinLength(2)]
    public required string Name { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [StrongPassword]
    public required string Password { get; set; }
}
