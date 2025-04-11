namespace DotNetService.Infrastructure.Exceptions
{
    public class BusinessException(string message) : Exception(message)
    {
    }
}