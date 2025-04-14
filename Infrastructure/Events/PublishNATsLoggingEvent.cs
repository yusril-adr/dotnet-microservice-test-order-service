using DotNetOrderService.Constants.Event;
using DotNetOrderService.Constants.Logger;
using DotNetOrderService.Infrastructure.Integrations.NATs;
using DotNetOrderService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetOrderService.Infrastructure.Events
{
    public class PublishNATsLoggingEvent : IAsyncActionFilter
    {
        public readonly ILogger _loggerIntegration;
        public readonly NATsIntegration _natsIntegration;

        public PublishNATsLoggingEvent(ILoggerFactory loggerFactory, NATsIntegration natsIntegration)
        {
            _loggerIntegration = loggerFactory.CreateLogger(LoggerConstant.INTEGRATION);
            _natsIntegration = natsIntegration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            var userId = isAuthenticated ? context.HttpContext.User.FindFirst("Id")?.Value.ToString() : Guid.Empty.ToString();
            var endpoint = context.HttpContext.Request.Path;
            var dataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            string subject = _natsIntegration.Subject(NATsEventModuleEnum.LOGGER, NATsEventActionEnum.DEBUG, NATsEventStatusEnum.INFO, NATsEventNATSType.JETSTREAM);

            await next();

            RunBackgroundProcess(context, subject, dataTime, userId, method, endpoint);
        }

        private void RunBackgroundProcess(ActionExecutingContext context, string subject, string dataTime, string userId, string method, string endpoint)
        {
            Utils.BackgroundProcessThreadAsync(async Task () =>
            {
                var data = new
                {
                    dataTime,
                    userId,
                    method,
                    endpoint,
                    actionArguments = context.ActionArguments,
                };

                await _natsIntegration.Publish<object>(subject, Utils.JsonSerialize(data));
            });
        }
    }
}