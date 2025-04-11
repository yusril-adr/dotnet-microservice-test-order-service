using DotNetService.Infrastructure.Dtos;

namespace DotNetService.Domain.Permission.Dtos
{
    public class PermissionQueryDto : QueryDto
    {
        public string Name { get; set; }
    }
}
