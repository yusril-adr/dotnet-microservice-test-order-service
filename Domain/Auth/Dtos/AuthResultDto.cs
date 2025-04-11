namespace DotNetService.Domain.Auth.Dtos
{
    public class AuthTokenResultDto
    {
        public DateTime ExpiredAt { get; set; }

        public required string Token { get; set; }
    }

    public class AuthPermissionResultDto(Models.Permission permission)
    {
        public Guid Id { get; set; } = permission.Id;

        public string Name { get; set; } = permission.Name;
    }

    public class AuthRoleResultDto(Models.Role role)
    {
        public Guid Id { get; set; } = role.Id;

        public string Name { get; set; } = role.Name;

        public virtual List<AuthPermissionResultDto> Permissions { get; set; } = role.RolePermissions.Select(per => new AuthPermissionResultDto(per.Permission)).ToList();
    }

    public class AccountResultDto(Models.User user)
    {
        public Guid Id { get; set; } = user.Id;
        public string Name { get; set; } = user.Name;
        public string Email { get; set; } = user.Email;
        public virtual AuthRoleResultDto Role { get; set; } = user.UserRoles.Count > 0 ? new AuthRoleResultDto(user.UserRoles.First().Role) : null;
    }
}