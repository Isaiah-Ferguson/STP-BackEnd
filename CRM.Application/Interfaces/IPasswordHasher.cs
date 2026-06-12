namespace CRM.Application.Interfaces;

public interface IPasswordHasher
{
    /// <summary>Generates a fresh random salt and the derived hash for a plaintext password.</summary>
    (string Hash, string Salt) HashPassword(string password);

    /// <summary>Verifies a plaintext password against a stored hash + salt in constant time.</summary>
    bool VerifyPassword(string password, string hash, string salt);
}
