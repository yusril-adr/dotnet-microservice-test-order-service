using System.ComponentModel.DataAnnotations;
using DotNetService.Infrastructure.Regexs;

namespace DotNetService.Domain.Auth.Dtos
{
    public class AuthSignInDto
    {
        [Required]
        [EmailAddress]
        [MinLength(5)]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(30)]
        [RegularExpression(AuthRegex.PASSWORD, ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
    }
}