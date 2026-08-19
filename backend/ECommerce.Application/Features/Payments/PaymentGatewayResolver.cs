using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Features.Payments;

public class PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways)
{
    public IPaymentGateway Resolve(string name)
        => gateways.FirstOrDefault(g => string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? throw new BusinessException($"Unsupported payment gateway '{name}'. Supported: {string.Join(", ", gateways.Select(g => g.Name))}.");
}
