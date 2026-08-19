namespace ECommerce.Application.Features.Payments;

// Gateway is "Razorpay" or "PayPal" - the customer picks at checkout.
public record InitiatePaymentDto(Guid OrderId, string Gateway);

public record InitiatePaymentResponseDto(Guid PaymentId, string Gateway, string GatewayOrderId, decimal Amount, string CurrencyCode, string? ClientKey, string? RedirectUrl);

public record ConfirmPaymentDto(Guid OrderId, string Gateway, string GatewayOrderId, string? GatewayPaymentId, string? Signature);

public record PaymentResultDto(Guid PaymentId, string PaymentStatus, string? TransactionId, string? FailureReason);
