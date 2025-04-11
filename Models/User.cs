using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    public class User : BaseModel
    {
        [MaxLength(150)]
        public string Name { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(150)]
        public string Password { get; set; }

        public virtual List<UserRole> UserRoles { get; set; }
    }
}
