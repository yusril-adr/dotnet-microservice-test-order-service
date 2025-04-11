namespace DotNetService.Infrastructure.Exceptions
{
    public class UnauthenticatedException(string message = "Unauthenticated") : Exception(message)
    {
    }
}