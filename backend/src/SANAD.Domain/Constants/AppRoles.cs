namespace SANAD.Domain.Constants;

/// <summary>
/// The only role names used by SANAD. Role checks and seeding must use these constants.
/// </summary>
public static class AppRoles
{
    public const string Manager = "Manager";
    public const string PayoutAuditor = "PayoutAuditor";
    public const string Treasurer = "Treasurer";
    public const string DocumentReviewer = "DocumentReviewer";
    public const string FieldOfficer = "FieldOfficer";
    public const string Donor = "Donor";

    public static readonly string[] All =
    [
        Manager,
        PayoutAuditor,
        Treasurer,
        DocumentReviewer,
        FieldOfficer,
        Donor
    ];
}
