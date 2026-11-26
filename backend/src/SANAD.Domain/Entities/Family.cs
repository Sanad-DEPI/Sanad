namespace SANAD.Domain.Entities;

public sealed class Family
{
    public Guid Id { get; set; }

    public required string HeadFullNameEnc { get; set; }

    public required string AddressEnc { get; set; }

    public required string PhoneEnc { get; set; }

    public required string Governorate { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedById { get; set; }

    public ICollection<FamilyMember> Members { get; set; } = new List<FamilyMember>();
}
