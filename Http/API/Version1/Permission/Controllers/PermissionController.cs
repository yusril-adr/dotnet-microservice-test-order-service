using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.Permission.Services;
using System.Net;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Constants.Permission;
using DotNetService.Domain.Permission.Dtos;
using DotNetService.Infrastructure.Helpers;

namespace DotNetService.Http.API.Version1.Permission.Controllers
{
    [Route("api/v1/permissions")]
    [ApiController]
    public class PermissionController(
        PermissionService permissionService
        ) : ControllerBase
    {
        private readonly PermissionService _permissionService = permissionService;

        [HttpGet()]
        [Permissions(PermissionConstant.PERMISSION_VIEW)]
        public async Task<ApiResponse> Index([FromQuery] PermissionQueryDto query)
        {
            var paginationResult = await _permissionService.Index(query);
            return new ApiResponsePagination<PermissionResultDto>(HttpStatusCode.OK, paginationResult);
        }

        [HttpGet("{id}")]
        [Permissions(PermissionConstant.PERMISSION_VIEW)]
        public async Task<ApiResponse> Show(Guid id)
        {
            var data = await _permissionService.DetailById(id);
            return new ApiResponseData<PermissionResultDto>(HttpStatusCode.OK, data);
        }

        [HttpPost()]
        [Permissions(PermissionConstant.PERMISSION_CREATE)]
        public async Task<ApiResponse> Store(PermissionCreateDto dataCreate)
        {
            await _permissionService.Create(dataCreate);
            return new ApiResponseData<PermissionResultDto>(HttpStatusCode.OK, null);
        }

        [HttpPut("{id}")]
        [Permissions(PermissionConstant.PERMISSION_UPDATE)]
        public async Task<ApiResponse> Update(Guid id, PermissionUpdateDto dataUpdate)
        {
            await _permissionService.Update(id, dataUpdate);
            return new ApiResponseData<PermissionResultDto>(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        [Permissions(PermissionConstant.PERMISSION_DELETE)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _permissionService.Delete(id);
            return new ApiResponseData<PermissionResultDto>(HttpStatusCode.OK, null);
        }
    }
}
