using SANAD.Domain.Constants;

namespace SANAD.UnitTests.Domain;

public sealed class AppRolesTests
{
    [Fact]
    public void All_ContainsEverySupportedRoleExactlyOnce()
    {
        var expected = new[]
        {
            AppRoles.Beneficiary,
            AppRoles.Donor,
            AppRoles.SuperAdmin,
            AppRoles.Manager,
            AppRoles.DocumentReviewer,
            AppRoles.FieldOfficer,
            AppRoles.PayoutAuditor,
            AppRoles.Treasurer
        };

        Assert.Equal(expected.Length, AppRoles.All.Length);
        Assert.All(expected, role => Assert.Contains(role, AppRoles.All));
        Assert.Equal(AppRoles.All.Length, AppRoles.All.Distinct().Count());
    }

    [Fact]
    public void PublicRegistration_ContainsOnlyDonorAndBeneficiary()
    {
        Assert.Equal([AppRoles.Beneficiary, AppRoles.Donor], AppRoles.PublicRegistration);
        Assert.All(AppRoles.PublicRegistration, role => Assert.Contains(role, AppRoles.All));
    }
}
