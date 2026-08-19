using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Payments;

public class PayPalGateway(HttpClient httpClient, IConfiguration configuration, ILogger<PayPalGateway> logger)
    : IPaymentGateway
{
    public string Name => "PayPal";

    public async Task<CreatePaymentResult> CreatePaymentAsync(
        Guid internalOrderId, decimal amount, string currencyCode, CancellationToken cancellationToken = default)
    {
        EnsureBaseAddress();
        await AuthenticateAsync(cancellationToken);

        var payload = new PayPalCreateOrderRequest("CAPTURE",
        [
            new PayPalPurchaseUnit(internalOrderId.ToString(), new PayPalAmount(currencyCode, amount.ToString("F2"))),
        ]);

        var response = await httpClient.PostAsJsonAsync("v2/checkout/orders", payload, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("PayPal order creation failed: {Status} {Body}", response.StatusCode, body);
            throw new InvalidOperationException($"PayPal order creation failed: {response.StatusCode}");
        }

        var order = await response.Content.ReadFromJsonAsync<PayPalOrderResponse>(cancellationToken)
            ?? throw new InvalidOperationException("PayPal returned an empty order response.");

        var approveLink = order.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href;
        return new CreatePaymentResult(order.Id, ClientKey: null, RedirectUrl: approveLink);
    }

    public async Task<ConfirmPaymentResult> ConfirmPaymentAsync(ConfirmPaymentRequest request, CancellationToken cancellationToken = default)
    {
        EnsureBaseAddress();
        await AuthenticateAsync(cancellationToken);

        // PayPal's Orders v2 API captures by order id, not a separate payment
        // id - the customer approves the order in PayPal's UI, then this
        // capture call is what actually moves the money.
        var response = await httpClient.PostAsync($"v2/checkout/orders/{request.GatewayOrderId}/capture", null, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("PayPal capture failed: {Status} {Body}", response.StatusCode, body);
            return new ConfirmPaymentResult(false, null, $"PayPal capture failed: {response.StatusCode}");
        }

        var capture = System.Text.Json.JsonSerializer.Deserialize<PayPalCaptureResponse>(body);
        var captureId = capture?.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault()?.Id;
        var isCompleted = string.Equals(capture?.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase);

        return isCompleted
            ? new ConfirmPaymentResult(true, captureId, null)
            : new ConfirmPaymentResult(false, null, $"PayPal order status: {capture?.Status}");
    }

    private void EnsureBaseAddress()
    {
        if (httpClient.BaseAddress is null)
        {
            var baseUrl = configuration["PayPal:BaseUrl"] ?? "https://api-m.sandbox.paypal.com";
            httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        }
    }

    // No token caching (see the note in DependencyInjection.cs) - simplest
    // correct thing for now; each Create/Confirm call fetches its own token.
    private async Task AuthenticateAsync(CancellationToken cancellationToken)
    {
        var clientId = configuration["PayPal:ClientId"] ?? throw new InvalidOperationException("PayPal:ClientId is not configured.");
        var clientSecret = configuration["PayPal:ClientSecret"] ?? throw new InvalidOperationException("PayPal:ClientSecret is not configured.");

        var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent([new KeyValuePair<string, string>("grant_type", "client_credentials")]),
            Headers = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))) },
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("PayPal auth failed: {Status} {Body}", response.StatusCode, body);
            throw new InvalidOperationException($"PayPal authentication failed: {response.StatusCode}");
        }

        var token = await response.Content.ReadFromJsonAsync<PayPalTokenResponse>(cancellationToken)
            ?? throw new InvalidOperationException("PayPal returned an empty token response.");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }

    private record PayPalTokenResponse([property: JsonPropertyName("access_token")] string AccessToken);

    private record PayPalCreateOrderRequest(
        [property: JsonPropertyName("intent")] string Intent,
        [property: JsonPropertyName("purchase_units")] List<PayPalPurchaseUnit> PurchaseUnits);

    private record PayPalPurchaseUnit(
        [property: JsonPropertyName("reference_id")] string ReferenceId,
        [property: JsonPropertyName("amount")] PayPalAmount Amount);

    private record PayPalAmount(
        [property: JsonPropertyName("currency_code")] string CurrencyCode,
        [property: JsonPropertyName("value")] string Value);

    private record PayPalOrderResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("links")] List<PayPalLink>? Links);

    private record PayPalLink(
        [property: JsonPropertyName("rel")] string Rel,
        [property: JsonPropertyName("href")] string Href);

    private record PayPalCaptureResponse(
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("purchase_units")] List<PayPalCapturedUnit>? PurchaseUnits);

    private record PayPalCapturedUnit([property: JsonPropertyName("payments")] PayPalPayments? Payments);

    private record PayPalPayments([property: JsonPropertyName("captures")] List<PayPalCapture>? Captures);

    private record PayPalCapture([property: JsonPropertyName("id")] string Id);
}
