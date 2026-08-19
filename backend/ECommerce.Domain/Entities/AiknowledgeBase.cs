using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class AiknowledgeBase : BaseEntity
{
    public Guid? ProductId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public byte[]? EmbeddingVector { get; set; }

    public string Category { get; set; } = null!;

    public string SourceType { get; set; } = null!;
    public virtual Product? Product { get; set; }
}
