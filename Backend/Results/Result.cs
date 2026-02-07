namespace Backend.Results;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; set; }

    public Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}

public class Result<T> : Result
{
    
    public T? Value { get; set; }
    
    public Result(bool IsSuccess, T? value, Error? error) : base(IsSuccess, error)
    {
        Value = value;
    }
    
    public static Result<T> Success(T value) => new Result<T>(true,value,null);
    public static Result<T> Failure(Error error) => new Result<T>(false,default,error);
}