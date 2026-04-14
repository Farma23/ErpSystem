using ErpSystem.Domain.Common;

namespace ErpSystem.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;

    private User() { }

    public static User Create(string email, string fullName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new Domain.Exceptions.DomainException("Email is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new Domain.Exceptions.DomainException("Password is required.");

        return new User
        {
            Email = email.ToLowerInvariant(),
            FullName = fullName,
            PasswordHash = passwordHash
        };
    }
}
