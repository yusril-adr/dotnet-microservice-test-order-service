using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetService.Domain.Role.Dtos
{
    public class RoleUpdateDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [JsonPropertyName("permission_ids")]
        public List<Guid> PermissionIds { get; set; }

        public static Models.Role Assign(RoleUpdateDto data)
        {
            Models.Role res = new()
            {
                Name = data.Name,
            };

            return res;
        }
    }
}