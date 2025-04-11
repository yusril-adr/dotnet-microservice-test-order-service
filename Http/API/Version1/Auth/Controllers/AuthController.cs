using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Domain.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using DotNetService.Infrastructure.Helpers;
using DotNetService.Domain.Auth.Dtos;

namespace DotNetService.Http.API.Version1.Auth.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(
        AuthService authService
        ) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        [AllowAnonymous]
        [HttpPost("sign-in")]
        [Consumes("application/json")]
        public async Task<ApiResponse> SignIn(AuthSignInDto authSignIn)
        {
            var authTokenResultDto = await _authService.SignIn(authSignIn);
            return new ApiResponseData<AuthTokenResultDto>(HttpStatusCode.OK, authTokenResultDto);
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        [AllowAnonymous]
        public async Task<ApiResponse> Register(AuthRegisterDto authRegister)
        {
            await _authService.Register(authRegister);
            return new ApiResponseData<object>(HttpStatusCode.OK, null);
        }

        [HttpGet("account")]
        public async Task<ApiResponse> Account()
        {
            var data = await _authService.Account();
            return new ApiResponseData<AccountResultDto>(HttpStatusCode.OK, data);
        }
    }
}
