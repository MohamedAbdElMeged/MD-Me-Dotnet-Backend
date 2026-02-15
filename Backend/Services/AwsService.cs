namespace Backend.Services;

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

public class AwsService(IAmazonS3 s3, IConfiguration configuration) : IAwsService
{
    public async Task<string> GenerateUploadPresignedUrl(string key, bool upload)
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
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(expiresMinutes)
        };
        if (upload)
        {
            request.Verb = HttpVerb.PUT;
            request.ContentType = "text/markdown";
        }

        var url = await s3.GetPreSignedURLAsync(request);
        return  url;
    }

    public async Task<bool> DeleteObject(string key)
    {
        var bucketName = configuration["AWS:S3:BucketName"];

        if (! await CheckFileExists(key))
        {
            return false;
        }
        try
        {

            var request = new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = NormalizeKey(key)
            };

            await s3.DeleteObjectAsync(request);
            return true;
        }
        catch (AmazonS3Exception e)
        {
            return false;
         
        }
    }

    private async Task<bool> CheckFileExists(string key)
    {
        var bucketName = configuration["AWS:S3:BucketName"];
        try
        {
            await s3.GetObjectMetadataAsync(bucketName, NormalizeKey(key));
            return true;
        }
        catch (AmazonS3Exception e)
        {
            if (string.Equals(e.ErrorCode, "NoSuchKey") || 
                string.Equals(e.ErrorCode, "NotFound"))
            {
                return false; 
            }

            throw;
        }
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