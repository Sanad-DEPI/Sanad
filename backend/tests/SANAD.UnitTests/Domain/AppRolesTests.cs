using SANAD.Domain.Constants;

namespace SANAD.UnitTests.Domain;

public sealed class AppRolesTests
{
    [Fact]
    public void All_ContainsEverySupportedRoleExactlyOnce()
    {
        var expected = new[]
        {
            AppRoles.Manager,
            AppRoles.PayoutAuditor,
            AppRoles.Treasurer,
            AppRoles.DocumentReviewer,
            AppRoles.FieldOfficer,
            AppRoles.Donor
        };

        Assert.Equal(expected.Length, AppRoles.All.Length);
        Assert.All(expected, role => Assert.Contains(role, AppRoles.All));
        Assert.Equal(AppRoles.All.Length, AppRoles.All.Distinct().Count());
    }
}
