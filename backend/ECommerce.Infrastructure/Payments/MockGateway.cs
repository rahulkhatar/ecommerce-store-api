using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Payments;

// Dev/test-only stand-in for a real gateway - no external API call, no
// credentials, always "succeeds". Registered only when the app is running
// in Development (see DependencyInjection.AddInfrastructureServices), so
// there's no way to reach this in production even if a client asked for it
// by name - PaymentGatewayResolver would just report it unsupported.
public class MockGateway : IPaymentGateway
{
    public string Name => "Mock";

    public Task<CreatePaymentResult> CreatePaymentAsync(
        Guid internalOrderId, decimal amount, string currencyCode, CancellationToken cancellationToken = default)
        => Task.FromResult(new CreatePaymentResult($"mock_order_{Guid.NewGuid():N}", ClientKey: null, RedirectUrl: null));

    public Task<ConfirmPaymentResult> ConfirmPaymentAsync(
        ConfirmPaymentRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new ConfirmPaymentResult(true, $"mock_txn_{Guid.NewGuid():N}", FailureReason: null));
}
