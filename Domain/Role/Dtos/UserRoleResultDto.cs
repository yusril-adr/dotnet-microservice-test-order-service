using DotNetService.Domain.User.Dtos;

namespace DotNetService.Domain.Role.Dtos
{
    public class UserRoleResultDto : Models.UserRole
    {
        public new UserResultDto User { get; set; }
        public new RoleResultDto Role { get; set; }

        public UserRoleResultDto(Models.UserRole userRole)
        {
            Id = userRole.Id;
            UserId = userRole.UserId;
            RoleId = userRole.RoleId;
            User = new UserResultDto(userRole.User);
            Role = new RoleResultDto(userRole.Role);
        }

        public static List<UserRoleResultDto> MapRepo(List<Models.UserRole> data)
        {
            return data?.Select(data => new UserRoleResultDto(data)).ToList();
        }
    }
}
