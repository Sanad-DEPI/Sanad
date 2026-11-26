namespace SANAD.Domain.Entities;

public sealed class FamilyMember
{
    public Guid Id { get; set; }

    public Guid FamilyId { get; set; }

    public required string FullNameEnc { get; set; }

    public required string NationalIdEnc { get; set; }

    public required string NationalIdHash { get; set; }

    public DateOnly BirthDate { get; set; }

    public required string Gender { get; set; }

    public required string Relation { get; set; }

    public bool IsProvider { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedById { get; set; }
}
