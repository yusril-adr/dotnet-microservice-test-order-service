using System.ComponentModel.DataAnnotations;

namespace DotnetOrderService.Domain.File.Dto
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
        public string Path { get; set; }
    }
}