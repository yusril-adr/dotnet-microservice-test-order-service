using DotnetOrderService.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetOrderService.Http.API.Health
{
    [Route("health")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController
    {
        [HttpGet]
        public ApiResponseData<object> Get()
        {
            return new ApiResponseData<object>(System.Net.HttpStatusCode.OK, new { message = "Service is running, and Healty" });
        }
    }
}
