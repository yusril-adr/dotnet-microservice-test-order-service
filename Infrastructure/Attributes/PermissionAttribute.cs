namespace DotNetOrderService.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class PermissionsAttribute : Attribute
    {
        public string[] Permissions { get; }

        public PermissionsAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }
    }
}
