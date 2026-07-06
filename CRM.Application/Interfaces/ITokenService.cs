using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Application.Interfaces;

public interface ITokenService
{
    /// <summary>Issues a signed JWT for the user along with its absolute expiry. Includes the linked staff role, if any, for management-write policies.</summary>
    (string Token, DateTime ExpiresAt) CreateToken(User user, StaffRole? staffRole = null);
}
