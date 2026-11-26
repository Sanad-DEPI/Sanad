namespace SANAD.Application.Features.Auth;

public sealed record RegistrationResponse(
    string Email,
    string AccountType,
    bool RequiresEmailVerification);
