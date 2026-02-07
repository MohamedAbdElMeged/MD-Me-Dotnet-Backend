namespace Backend.Results;

public sealed record Error(string Code, ErrorType ErrorType, string Message);
