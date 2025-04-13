
namespace DotnetOrderService.Infrastructure.Regexs
{
    public static class AuthRegex
    {
        public const string PASSWORD = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"; // Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.
    }
}