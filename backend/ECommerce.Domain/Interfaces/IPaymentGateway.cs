namespace ECommerce.Domain.Interfaces;

public record CreatePaymentResult(string GatewayOrderId, string? ClientKey, string? RedirectUrl);

public record ConfirmPaymentRequest(string GatewayOrderId, string? GatewayPaymentId, string? Signature);

public record ConfirmPaymentResult(bool Success, string? ProviderTransactionId, string? FailureReason);

// One implementation per supported gateway (Razorpay, PayPal - see
// ECommerce.Infrastructure/Payments). The customer picks which one to use
// at checkout; PaymentGatewayName below is what both the Payments table's
// PaymentGateway column and the client's "gateway" request field use to
// select an implementation, so keep it stable ("Razorpay"/"PayPal").
public interface IPaymentGateway
{
    string Name { get; }

    Task<CreatePaymentResult> CreatePaymentAsync(Guid internalOrderId, decimal amount, string currencyCode, CancellationToken cancellationToken = default);

    Task<ConfirmPaymentResult> ConfirmPaymentAsync(ConfirmPaymentRequest request, CancellationToken cancellationToken = default);
}
