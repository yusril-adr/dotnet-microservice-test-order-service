namespace DotnetOrderService.Infrastructure.Exceptions
{
    public class ServiceUnavailableException(string message = "ServiceUnavailableException") : Exception(message)
    {
    }
}