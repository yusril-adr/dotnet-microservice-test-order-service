using DotNetService.Domain.Permission.Dtos;

namespace DotNetService.Domain.Role.Dtos
{
    public class RoleResultDto : Models.Role
    {
        public new List<UserRoleResultDto> UserRoles { get; set; }
        public List<PermissionResultDto> Permissions { get; set; }

        public RoleResultDto(Models.Role role)
        {
            Id = role.Id;
            Name = role.Name;
            Key = role.Key;
            Permissions = role.RolePermissions?.Count > 0
              ? PermissionResultDto.MapRepo(role.RolePermissions?.Select(data => data.Permission).ToList())
              : null;
        }

        public static List<RoleResultDto> MapRepo(List<Models.Role> data)
        {
            return data?.Select(data => new RoleResultDto(data)).ToList();
        }
    }

    public class RoleItem(Models.Role roleRepository)
    {
        public Guid Id { get; set; } = roleRepository.Id;
        public string Name { get; set; } = roleRepository.Name;

        public static List<RoleItem> MapRepo(List<Models.Role> roles)
        {
            var roleMapped = new List<RoleItem>();
            if (roles == null)
            {
                return [];
            }

            foreach (Models.Role role in roles)
            {
                roleMapped.Add(new RoleItem(role));
            }

            return roleMapped;
        }
    }
}