using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.Role.Services;
using System.Net;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Constants.Permission;
using DotNetService.Domain.Role.Dtos;
using DotNetService.Infrastructure.Helpers;

namespace DotNetService.Http.API.Version1.Role.Controllers
{
    [Route("api/v1/roles")]
    [ApiController]
    public class RoleController(
        RoleService roleService
        ) : ControllerBase
    {
        private readonly RoleService _roleService = roleService;

        [HttpGet()]
        [Permissions(PermissionConstant.ROLE_VIEW)]
        public async Task<ApiResponse> Index([FromQuery] RoleQueryDto query)
        {
            var paginationResult = await _roleService.Index(query);
            return new ApiResponsePagination<RoleResultDto>(HttpStatusCode.OK, paginationResult);
        }

        [HttpGet("{id}")]
        [Permissions(PermissionConstant.ROLE_VIEW)]
        public async Task<ApiResponse> Show(Guid id)
        {
            var role = await _roleService.DetailById(id);
            return new ApiResponseData<RoleResultDto>(HttpStatusCode.OK, role);
        }

        [HttpPost()]
        [Permissions(PermissionConstant.ROLE_CREATE)]
        public async Task<ApiResponse> Store(RoleCreateDto dataCreate)
        {
            await _roleService.Create(dataCreate);
            return new ApiResponseData<RoleResultDto>(HttpStatusCode.OK, null);
        }

        [HttpPut("{id}")]
        [Permissions(PermissionConstant.ROLE_UPDATE)]
        public async Task<ApiResponse> Update(Guid id, RoleUpdateDto dataUpdate)
        {
            await _roleService.Update(id, dataUpdate);
            return new ApiResponseData<RoleResultDto>(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        [Permissions(PermissionConstant.ROLE_DELETE)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _roleService.Delete(id);
            return new ApiResponseData<RoleResultDto>(HttpStatusCode.OK, null);
        }
    }
}
