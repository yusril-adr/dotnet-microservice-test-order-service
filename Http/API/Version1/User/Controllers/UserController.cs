using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Domain.User.Services;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Constants.Permission;
using DotNetService.Domain.User.Dtos;
using DotNetService.Infrastructure.Helpers;

namespace DotNetService.Http.API.Version1.User.Controllers
{
    [Route("api/v1/users")]
    [ApiController]

    public class UserController(
        UserService userService
        ) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet()]
        [Permissions(PermissionConstant.USER_VIEW)]
        public async Task<ApiResponse> Index([FromQuery] UserQueryDto query)
        {
            var paginationResult = await _userService.Index(query);
            return new ApiResponsePagination<UserResultDto>(HttpStatusCode.OK, paginationResult);
        }

        [HttpGet("{id}")]
        [Permissions(PermissionConstant.USER_VIEW)]
        public async Task<ApiResponse> Show(Guid id)
        {
            Models.User data = await _userService.Detail(id);
            return new ApiResponseData<Models.User>(HttpStatusCode.OK, data);
        }

        [HttpPost()]
        [Permissions(PermissionConstant.USER_CREATE)]
        public async Task<ApiResponse> Store(UserCreateDto dataCreate)
        {
            await _userService.Create(dataCreate);
            return new ApiResponseData<Models.User>(HttpStatusCode.OK, null);
        }

        [HttpPut("{id}")]
        [Permissions(PermissionConstant.USER_UPDATE)]
        public async Task<ApiResponse> Update(Guid id, UserUpdateDto dataUpdate)
        {
            await _userService.Update(id, dataUpdate);
            return new ApiResponseData<Models.User>(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        [Permissions(PermissionConstant.USER_DELETE)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _userService.Delete(id);
            return new ApiResponseData<Models.User>(HttpStatusCode.OK, null);
        }
    }
}
