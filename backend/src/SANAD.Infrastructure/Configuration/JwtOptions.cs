namespace SANAD.Infrastructure.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "SANAD";
    public string Audience { get; set; } = "SANAD.Client";
    public int ExpiryMinutes { get; set; } = 60;
}
