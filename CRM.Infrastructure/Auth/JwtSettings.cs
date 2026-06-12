namespace CRM.Infrastructure.Auth;

/// <summary>Bound from the "Jwt" section of configuration.</summary>
public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ShinyStarCRM";
    public string Audience { get; set; } = "ShinyStarCRM";
    public int ExpiryMinutes { get; set; } = 480; // 8 hours
}
