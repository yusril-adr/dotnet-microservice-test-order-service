namespace DotNetOrderService.Infrastructure.Exceptions
{
    public class UnauthenticatedException(string message = "Unauthenticated") : Exception(message)
    {
    }
}