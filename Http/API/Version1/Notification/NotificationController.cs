using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Domain.Notification.Services;
using DotNetService.Infrastructure.Helpers;
using DotNetService.Domain.Notification.Dtos;

namespace DotNetService.Http.API.Version1.Notification
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationController(
        NotificationService notificationService
        ) : ControllerBase
    {
        private readonly NotificationService _notificationService = notificationService;

        [HttpGet()]
        public async Task<ApiResponse> Index([FromQuery] NotificationQueryDto query)
        {
            var paginationResult = await _notificationService.Index(query);
            return new ApiResponsePagination<NotificationResultDto>(HttpStatusCode.OK, paginationResult);
        }

        [HttpGet("has-unread")]
        public async Task<ApiResponse> HasUnread()
        {
            var data = await _notificationService.UserHasUnreadNotification();
            return new ApiResponseData<bool>(HttpStatusCode.OK, data);
        }

        [HttpPatch("read/{id}")]
        public async Task<ApiResponse> Read(Guid id)
        {
            await _notificationService.ReadNotificationById(id);
            return new ApiResponseData<Models.Notification>(HttpStatusCode.OK, null);
        }

        [HttpPatch("read-all")]
        public async Task<ApiResponse> ReadAll()
        {
            await _notificationService.ReadAllNotification();
            return new ApiResponseData<Models.Notification>(HttpStatusCode.OK, null);
        }
    }
}
