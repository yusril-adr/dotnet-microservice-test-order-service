using DotNetService.Infrastructure.Dtos;

namespace DotNetService.Domain.Role.Dtos
{
    public class RoleQueryDto : QueryDto
    {
        public string Name { get; set; }
    }
}