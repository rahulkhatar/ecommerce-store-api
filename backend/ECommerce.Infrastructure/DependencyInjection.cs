using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Authentication;
using ECommerce.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // Further concrete registrations (email, AI services) are added here as
    // each vertical slice introduces them.
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Typed HttpClients so each gateway gets its own pooled handler (via
        // IHttpClientFactory) instead of a raw `new HttpClient()` per call.
        // Both are also registered as IPaymentGateway so the Application
        // layer can resolve "Razorpay"/"PayPal" by name (see PaymentGatewayResolver).
        services.AddHttpClient<RazorpayGateway>();
        services.AddHttpClient<PayPalGateway>();
        services.AddScoped<IPaymentGateway>(sp => sp.GetRequiredService<RazorpayGateway>());
        services.AddScoped<IPaymentGateway>(sp => sp.GetRequiredService<PayPalGateway>());

        return services;
    }
}
