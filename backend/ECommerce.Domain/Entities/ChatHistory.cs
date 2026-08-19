using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class ChatHistory : BaseEntity
{
    public Guid CustomerId { get; set; }

    public string MessageRole { get; set; } = null!;

    public string MessageContent { get; set; } = null!;

    public int? ResponseTime { get; set; }

    public int? TokensUsed { get; set; }

    public Guid SessionId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
}
