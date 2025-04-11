using System.ComponentModel.DataAnnotations;

namespace DotNetService.Domain.Permission.Dtos
{
    public class PermissionUpdateDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        public static Models.Permission Assign(PermissionUpdateDto data)
        {
            Models.Permission res = new()
            {
                Name = data.Name,
            };

            return res;
        }
    }
}