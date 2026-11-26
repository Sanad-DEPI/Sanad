namespace SANAD.Domain.Constants;

/// <summary>
/// The only role names used by SANAD. Role checks and seeding must use these constants.
/// </summary>
public static class AppRoles
{
    public const string Beneficiary = "Beneficiary";
    public const string Donor = "Donor";
    public const string SuperAdmin = "SuperAdmin";
    public const string Manager = "Manager";
    public const string DocumentReviewer = "DocumentReviewer";
    public const string FieldOfficer = "FieldOfficer";
    public const string PayoutAuditor = "PayoutAuditor";
    public const string Treasurer = "Treasurer";

    public static readonly string[] PublicRegistration = [Beneficiary, Donor];

    public static readonly string[] All =
    [
        Beneficiary,
        Donor,
        SuperAdmin,
        Manager,
        DocumentReviewer,
        FieldOfficer,
        PayoutAuditor,
        Treasurer
    ];
}
