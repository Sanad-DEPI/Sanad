namespace SANAD.Infrastructure.Configuration;

public sealed class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    public int LifetimeDays { get; set; } = 7;
}
