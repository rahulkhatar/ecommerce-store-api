using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Payments;

public class RazorpayGateway(HttpClient httpClient, IConfiguration configuration, ILogger<RazorpayGateway> logger)
    : IPaymentGateway
{
    public string Name => "Razorpay";

    public async Task<CreatePaymentResult> CreatePaymentAsync(
        Guid internalOrderId, decimal amount, string currencyCode, CancellationToken cancellationToken = default)
    {
        var (keyId, keySecret) = GetCredentials();
        httpClient.BaseAddress ??= new Uri("https://api.razorpay.com/");
        httpClient.DefaultRequestHeaders.Authorization = BasicAuth(keyId, keySecret);

        // Razorpay amounts are in the smallest currency unit (paise for INR).
        var payload = new RazorpayCreateOrderRequest((long)Math.Round(amount * 100), currencyCode, internalOrderId.ToString());
        var response = await httpClient.PostAsJsonAsync("v1/orders", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Razorpay order creation failed: {Status} {Body}", response.StatusCode, body);
            throw new InvalidOperationException($"Razorpay order creation failed: {response.StatusCode}");
        }

        var order = await response.Content.ReadFromJsonAsync<RazorpayOrderResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Razorpay returned an empty order response.");

        // ClientKey is the public key_id - safe to hand to the frontend for
        // Razorpay Checkout.js, unlike key_secret which never leaves the server.
        return new CreatePaymentResult(order.Id, keyId, RedirectUrl: null);
    }

    public Task<ConfirmPaymentResult> ConfirmPaymentAsync(ConfirmPaymentRequest request, CancellationToken cancellationToken = default)
    {
        // Razorpay's checkout flow returns razorpay_order_id, razorpay_payment_id,
        // and razorpay_signature to the client; verifying the signature locally
        // (HMAC-SHA256 over "order_id|payment_id" keyed with key_secret) is
        // Razorpay's documented way to confirm the payment without another API
        // call - see https://razorpay.com/docs/payments/payment-gateway/webhooks/validate-test/
        var (_, keySecret) = GetCredentials();

        if (string.IsNullOrEmpty(request.GatewayPaymentId) || string.IsNullOrEmpty(request.Signature))
        {
            return Task.FromResult(new ConfirmPaymentResult(false, null, "Missing payment id or signature."));
        }

        var payload = $"{request.GatewayOrderId}|{request.GatewayPaymentId}";
        var expectedSignature = Convert.ToHexStringLower(
            HMACSHA256.HashData(Encoding.UTF8.GetBytes(keySecret), Encoding.UTF8.GetBytes(payload)));

        var isValid = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedSignature),
            Encoding.UTF8.GetBytes(request.Signature.ToLowerInvariant()));

        return Task.FromResult(isValid
            ? new ConfirmPaymentResult(true, request.GatewayPaymentId, null)
            : new ConfirmPaymentResult(false, null, "Signature verification failed."));
    }

    private (string KeyId, string KeySecret) GetCredentials()
    {
        var keyId = configuration["Razorpay:KeyId"] ?? throw new InvalidOperationException("Razorpay:KeyId is not configured.");
        var keySecret = configuration["Razorpay:KeySecret"] ?? throw new InvalidOperationException("Razorpay:KeySecret is not configured.");
        return (keyId, keySecret);
    }

    private static AuthenticationHeaderValue BasicAuth(string keyId, string keySecret)
        => new("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{keySecret}")));

    private record RazorpayCreateOrderRequest(
        [property: JsonPropertyName("amount")] long Amount,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("receipt")] string Receipt);

    private record RazorpayOrderResponse([property: JsonPropertyName("id")] string Id);
}
