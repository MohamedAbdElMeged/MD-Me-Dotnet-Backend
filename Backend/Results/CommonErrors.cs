namespace Backend.Results;

public static class CommonErrors
{

    public static Error NotFoundError(string model)
    {
        if (char.IsLower(model[0]))
        {
            model = char.ToUpper(model[0]) + model.Substring(1);
        }
        return new Error(
            Code: "NOT_FOUND",
            ErrorType: ErrorType.NotFound,
            Message: $"{model} not found"
        );
    }
}