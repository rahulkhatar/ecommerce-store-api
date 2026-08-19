namespace ECommerce.Domain.Interfaces;

// Declared in Domain (not Infrastructure, despite skills-clean-architecture.md's
// example placing IEmailService/IPaymentGateway/IAIService inside Infrastructure
// itself) - Infrastructure's csproj references Application, so an Application
// handler could not depend on an Infrastructure-declared interface without a
// circular project reference. Concrete implementation lives in
// ECommerce.Infrastructure/Authentication.
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
