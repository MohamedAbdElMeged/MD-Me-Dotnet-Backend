namespace Backend.Results;

public record Error(ErrorType errorType, string Message, string Code);