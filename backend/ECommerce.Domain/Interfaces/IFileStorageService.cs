namespace ECommerce.Domain.Interfaces;

public interface IFileStorageService
{
    // Returns a relative URL path (e.g. "/uploads/products/{guid}.jpg") - the
    // frontend prepends its API base URL to render it, so nothing here bakes
    // in a specific host/domain.
    Task<string> SaveProductImageAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
}
