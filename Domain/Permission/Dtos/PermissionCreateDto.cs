using System.ComponentModel.DataAnnotations;

namespace DotNetService.Domain.Permission.Dtos
{
    public class PermissionCreateDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Key { get; set; }

        public static Models.Permission Assign(PermissionCreateDto data)
        {
            Models.Permission res = new()
            {
                Name = data.Name,
                Key = data.Key
            };

            return res;
        }
    }
}