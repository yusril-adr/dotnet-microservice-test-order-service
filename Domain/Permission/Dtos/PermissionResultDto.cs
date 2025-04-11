namespace DotNetService.Domain.Permission.Dtos
{
    public class PermissionResultDto : Models.Permission
    {
        public PermissionResultDto(Models.Permission permission)
        {
            Id = permission.Id;
            Name = permission.Name;
            Key = permission.Key;
            CreatedAt = permission.CreatedAt;
            UpdatedAt = permission.UpdatedAt;
        }

        public static List<PermissionResultDto> MapRepo(List<Models.Permission> data)
        {
            return data?.Select(data => new PermissionResultDto(data)).ToList();
        }
    }
}
