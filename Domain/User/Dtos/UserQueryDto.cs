

using DotNetService.Infrastructure.Dtos;

namespace DotNetService.Domain.User.Dtos
{
    public class UserQueryDto : QueryDto
    {
        public string Email { get; set; }
    }
}