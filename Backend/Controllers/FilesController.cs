using System.Security.Cryptography;
using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Backend.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IAmazonS3 _s3Client;

    public FilesController(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    // encrypt the file content only 
    [HttpPost("files")]
    public async Task<IActionResult> UploadFile([FromBody] FileRequestDto fileRequestDto, IWebHostEnvironment env)
    {
        var putObjectRequest = new PutObjectRequest();
     
        var uploadsDir = Path.Combine(env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);
        var filePath = Path.Combine(uploadsDir, fileRequestDto.FileName);
        const string encryptionKey = "Hamada@123";
        var encryptedContent = Encrypt(fileRequestDto.FileContent,encryptionKey);
        await  System.IO.File.WriteAllTextAsync(filePath, encryptedContent.ToString());
        var decryptedContent = DecryptString(encryptedContent.ToString(),encryptionKey);
        await  System.IO.File.AppendAllTextAsync(filePath, decryptedContent.ToString());
        


        // putObjectRequest.ContentBody = ;
        putObjectRequest.BucketName = "s3-demo-try-md-me";
        putObjectRequest.ContentType = "text/markdown";
        putObjectRequest.Key = fileRequestDto.FileName;
        // putObjectRequest.FilePath = $"/FirstUploads/{fileRequestDto.FileName}";
        await _s3Client.PutObjectAsync(putObjectRequest);
        return Ok(fileRequestDto);
    }

    [HttpGet("list-buckets")]
    public async Task<IActionResult> ListBuckets()
    {
        var data = await _s3Client.ListBucketsAsync();
        var buckets = data.Buckets.Select(b => { return b.BucketName; });
        return Ok(buckets);
    }

    private object Encrypt(string fileContent, string encryptionKey)
    {
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(fileContent);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    private string DecryptString(string cipherTextBase64, string encryptionKey)
    {
        var fullCipher = Convert.FromBase64String(cipherTextBase64);
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var iv = new byte[16];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        var cipherBytes = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
    [HttpGet("")]
    public async Task<IActionResult> GetFiles()
    {
        return Ok();
    }
}