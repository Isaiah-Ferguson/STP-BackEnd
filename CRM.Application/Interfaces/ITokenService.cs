using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface ITokenService
{
    /// <summary>Issues a signed JWT for the user along with its absolute expiry.</summary>
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
