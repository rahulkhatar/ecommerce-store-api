using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // Concrete registrations (JWT/password hashing, payment gateway, email,
    // AI services) are added here as each vertical slice introduces them -
    // see the Auth slice for the first ones.
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
