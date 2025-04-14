using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetOrderService.Domain.File.Services;
using DotNetOrderService.Infrastructure.Helpers;
using DotNetOrderService.Domain.File.Dto;

namespace DotNetOrderService.Http.API.Version1.File.Controllers
{
    [Route("api/v1/files")]
    [ApiController]
    public class FileController(FileService FileService) : ControllerBase
    {
        private readonly FileService _fileService = FileService;

        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<ApiResponseData<FileDto>> UploadAsync([FromForm] FileUploadDto request)
        {
            var data = await _fileService.UploadAsync(request);
            return new ApiResponseData<FileDto>(HttpStatusCode.OK, data);
        }

        [HttpPost("download")]
        public ApiResponseData<FileDto> Download([FromBody] FileDowloadDto request)
        {
            var data = _fileService.Download(request);
            return new ApiResponseData<FileDto>(HttpStatusCode.OK, data);
        }
    }
}
