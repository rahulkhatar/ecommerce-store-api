namespace ECommerce.Application.Features.Inquiries;

public record CreateInquiryDto(string Name, string Email, string? Phone, string Subject, string Message);

public record InquiryDto(Guid Id, string Name, string Email, string? Phone, string Subject, string Message, bool IsResolved, DateTime? CreatedAt);
