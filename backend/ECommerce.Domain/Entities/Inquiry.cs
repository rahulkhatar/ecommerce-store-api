namespace ECommerce.Domain.Entities;

public class Inquiry : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool? IsResolved { get; set; }
}
