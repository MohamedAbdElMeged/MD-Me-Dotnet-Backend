namespace Backend.Services;

public interface IAwsService
{   
    //generate presigned put url
    // generate presigned get url

    public Task<string> GenerateUploadPresignedUrl(string key);
}