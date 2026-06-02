using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class StrongPasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var password = (string?)value ?? "";

        if (password.Length < 8)
        {
            return new ValidationResult("Password must be at least 8 characters.");
        }

        return ValidationResult.Success;
    }
}
