using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.AI;
using ECommerce.Infrastructure.Authentication;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Payments;
using ECommerce.Infrastructure.Search;
using ECommerce.Infrastructure.Storage;
using Elastic.Clients.Elasticsearch;
using Elastic.SemanticKernel.Connectors.Elasticsearch;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using System.ClientModel;

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

        // Redis - cache-aside for product reads (see ProductCacheKeys / the
        // Get*Query handlers in Application). Falls back to always-hit-the-DB
        // if Redis is unreachable (see RedisCacheService).
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"] ?? "localhost:6379";
            options.InstanceName = "ecommerce:";
        });
        services.AddSingleton<ICacheService, RedisCacheService>();

        // Elasticsearch hybrid (keyword + vector) product search, using
        // Microsoft.Extensions.VectorData's IKeywordHybridSearchable via the
        // Elastic-published connector. Registered as singletons: the client,
        // embedding generator, and collection are all stateless/thread-safe,
        // and re-resolving them per-request would just reconnect pointlessly.
        services.AddSingleton(sp =>
        {
            var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
            return new ElasticsearchClient(new Uri(url));
        });

        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var apiKey = configuration["OpenAI:ApiKey"] ?? "your-openai-api-key";
            var model = configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
            var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
            return openAiClient.GetEmbeddingClient(model).AsIEmbeddingGenerator();
        });

        services.AddSingleton(sp => new ElasticsearchCollection<Guid, ProductSearchDocument>(
            sp.GetRequiredService<ElasticsearchClient>(),
            name: "products",
            ownsClient: false,
            options: new ElasticsearchCollectionOptions
            {
                EmbeddingGenerator = sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>(),
            }));

        services.AddSingleton<IProductSearchService, ElasticsearchProductSearchService>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Scoped (not singleton): it depends on ISender/IChatHistoryRepository,
        // which are themselves scoped - a singleton here would capture a
        // request-scoped dependency past its lifetime.
        services.AddScoped<IShoppingAssistantService, OpenAiShoppingAssistantService>();

        return services;
    }
}
