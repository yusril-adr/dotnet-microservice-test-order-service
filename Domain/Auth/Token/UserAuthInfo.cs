namespace DotNetService.Domain.Auth.Token
{
    public class UserAuthInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual List<string> Permissions { get; set; }
    }
}