namespace ECommerce.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    // Nullable to match the DB schema (database/migrations/001_InitialSchema.sql
    // defines these with a DEFAULT but not NOT NULL) - always populated in
    // practice via the DB default or EF SaveChanges, but the type reflects
    // what the column actually allows.
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}
