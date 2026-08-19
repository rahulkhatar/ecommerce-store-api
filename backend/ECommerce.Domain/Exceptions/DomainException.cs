namespace ECommerce.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class NotFoundException(string message) : DomainException(message);

public sealed class ValidationException(string message, IReadOnlyList<string> errors) : DomainException(message)
{
    public IReadOnlyList<string> Errors { get; } = errors;
}

public sealed class BusinessException(string message) : DomainException(message);
