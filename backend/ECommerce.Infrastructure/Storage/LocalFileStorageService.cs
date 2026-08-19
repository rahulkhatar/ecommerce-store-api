using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace ECommerce.Infrastructure.Storage;

// Saves to wwwroot/uploads/products on local disk, served back out via
// app.UseStaticFiles() in Program.cs. This is a dev/single-server approach -
// wwwroot is bind-mounted (./backend:/app in docker-compose.yml) so uploads
// survive container restarts, but a real multi-instance production
// deployment would want this backed by object storage (S3/Azure Blob)
// instead; IFileStorageService is the seam to swap that in later without
// touching callers.
public class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif",
    };

    private static readonly Dictionary<string, string> ExtensionByContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
        ["image/gif"] = ".gif",
    };

    private const long MaxBytes = 10 * 1024 * 1024; // matches MAX_FILE_SIZE_MB in .env.example

    public async Task<string> SaveProductImageAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (!AllowedContentTypes.Contains(contentType))
        {
            throw new BusinessException($"Unsupported image type '{contentType}'. Allowed: jpeg, png, webp, gif.");
        }

        if (content.Length > MaxBytes)
        {
            throw new BusinessException($"Image exceeds the {MaxBytes / (1024 * 1024)}MB limit.");
        }

        var uploadsDir = Path.Combine(environment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadsDir);

        var storedName = $"{Guid.NewGuid()}{ExtensionByContentType[contentType]}";
        var fullPath = Path.Combine(uploadsDir, storedName);

        await using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return $"/uploads/products/{storedName}";
    }
}
