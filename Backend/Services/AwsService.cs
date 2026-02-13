namespace Backend.Services;

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

public class AwsService(IAmazonS3 s3, IConfiguration configuration) : IAwsService
{
    public async Task<string> GenerateUploadPresignedUrl(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required", nameof(key));

        var normalizedKey = NormalizeKey(key);
        var bucketName = configuration["AWS:S3:BucketName"];
        if (string.IsNullOrWhiteSpace(bucketName))
            throw new InvalidOperationException("Missing configuration value AWS:S3:BucketName");

        var expiresMinutes = configuration.GetValue<int?>("AWS:S3:UploadUrlExpiresMinutes") ?? 15;
        if (expiresMinutes <= 0)
            expiresMinutes = 15;

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = normalizedKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(expiresMinutes)
        };

        var url = await s3.GetPreSignedURLAsync(request);
        return  url;
    }

    private static string NormalizeKey(string key)
    {
        var normalized = key.Trim().Replace('\\', '/');
        while (normalized.StartsWith('/'))
            normalized = normalized.Substring(1);

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Key is invalid", nameof(key));

        if (normalized.Contains(".."))
            throw new ArgumentException("Key cannot contain '..'", nameof(key));

        return normalized;
    }
}