namespace DotNetService.Infrastructure.Exceptions;

public class UnprocessableEntityException(string message = "Unprocessable Entity") : Exception(message)
{
    public int StatusCode { get; } = 422;
}