using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetService.Domain.Role.Dtos
{
    public class RoleCreateDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Key { get; set; }

        [Required]
        [JsonPropertyName("permission_ids")]
        public List<Guid> PermissionIds { get; set; }

        public static Models.Role Assign(RoleCreateDto data)
        {
            Models.Role res = new()
            {
                Name = data.Name,
                Key = data.Key
            };

            return res;
        }
    }
}