namespace document_generator.Infrastructure.Helpers;

public class ResponseBuilder
{
    public static ResponseFormat SuccessResponse(string message, dynamic data)
    {
        return new ResponseFormat
        {
            StatusCode = 200,
            Success = true,
            Message = message,
            Data = data
        };
    }
    
    public static ResponseFormat ErrorResponse(int statusCode, string message, dynamic errors)
    {
        return new ResponseFormat
        {
            StatusCode = statusCode,
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}