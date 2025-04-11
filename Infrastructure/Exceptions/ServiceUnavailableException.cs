namespace DotNetService.Infrastructure.Exceptions
{
    public class ServiceUnavailableException(string message = "ServiceUnavailableException") : Exception(message)
    {
    }
}