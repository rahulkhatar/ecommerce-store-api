using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Authentication;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Payments;
using ECommerce.Infrastructure.Storage;
using ECommerce.Infrastructure.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // Further concrete registrations (email, AI services) are added here as
    // each vertical slice introduces them.
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
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

        // Dev/test-only "Mock" gateway so checkout can be exercised end to
        // end without real Razorpay/PayPal credentials - gated on the
        // environment, not a config flag, so it can't accidentally ship
        // enabled in production regardless of what appsettings says.
        if (isDevelopment)
        {
            services.AddScoped<IPaymentGateway, MockGateway>();
        }

        // In-memory cache-aside for product reads (see ProductCacheKeys / the
        // Get*Query handlers in Application) - see DistributedMemoryCacheService's
        // own comment for why this is in-process rather than a separate Redis
        // service.
        services.AddDistributedMemoryCache();
        services.AddSingleton<ICacheService, DistributedMemoryCacheService>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Scoped (not singleton): it depends on ISender/IChatHistoryRepository,
        // which are themselves scoped - a singleton here would capture a
        // request-scoped dependency past its lifetime.
        services.AddScoped<IShoppingAssistantService, OpenAiShoppingAssistantService>();

        return services;
    }
}
