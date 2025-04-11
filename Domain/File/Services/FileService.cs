using DotNetService.Domain.File.Dto;
using DotNetService.Infrastructure.Shareds;

namespace DotNetService.Domain.File.Services
{
    public class FileService(StorageService storageService)
    {
        private readonly StorageService _StorageService = storageService;

        public async Task<FileDto> UploadAsync(FileUploadDto request)
        {
            return await _StorageService.UploadAsync(request);
        }

        public FileDto Download(FileDowloadDto request)
        {
            return _StorageService.Download(request.Key);
        }
    }
}