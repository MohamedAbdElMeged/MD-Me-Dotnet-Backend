namespace Backend.Services;

public interface IAwsService
{   


    public Task<string> GenerateUploadPresignedUrl(string key, bool upload);

    public Task<bool> DeleteObject(string key);

}