using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Storage;

// Cloudflare R2 (S3-compatible object storage) - used instead of
// LocalFileStorageService whenever R2 credentials are configured (see
// DependencyInjection). Render's own disk is ephemeral, so anything an
// admin uploads via SaveProductImageAsync needs to live somewhere that
// survives a redeploy; R2's free tier (10GB, no egress fees) covers that
// without a recurring cost. Unlike the local version, this returns an
// absolute URL (R2's own public domain) rather than a path relative to this
// API - resolveImageUrl on the frontend already passes absolute URLs
// through unchanged, so no frontend change was needed for this.
public class R2FileStorageService : IFileStorageService
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

    private const long MaxBytes = 10 * 1024 * 1024;

    private readonly AmazonS3Client client;
    private readonly string bucketName;
    private readonly string publicUrl;

    public R2FileStorageService(IConfiguration configuration)
    {
        var accountId = configuration["R2:AccountId"]!;
        var accessKey = configuration["R2:AccessKeyId"]!;
        var secretKey = configuration["R2:SecretAccessKey"]!;
        bucketName = configuration["R2:BucketName"]!;
        publicUrl = configuration["R2:PublicUrl"]!.TrimEnd('/');

        client = new AmazonS3Client(
            new BasicAWSCredentials(accessKey, secretKey),
            new AmazonS3Config
            {
                ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true,
            });
    }

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

        var key = $"products/{Guid.NewGuid()}{ExtensionByContentType[contentType]}";

        await client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType,
        }, cancellationToken);

        return $"{publicUrl}/{key}";
    }
}
