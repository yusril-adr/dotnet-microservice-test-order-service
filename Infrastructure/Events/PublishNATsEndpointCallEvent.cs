using DotNetOrderService.Constants.Event;
using DotNetOrderService.Constants.Logger;
using DotNetOrderService.Infrastructure.Integrations.NATs;
using DotNetOrderService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetOrderService.Infrastructure.Events
{
    public class PublishNATsEndpointCallEvent(
        NATsIntegration natsIntegration,
        ILoggerFactory loggerFactory
    ) : IAsyncActionFilter
    {
        public readonly ILogger _loggerIntegration = loggerFactory.CreateLogger(LoggerConstant.INTEGRATION);
        public readonly NATsIntegration _natsIntegration = natsIntegration;

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            var userId = isAuthenticated ? context.HttpContext.User.FindFirst("Id")?.Value.ToString() : Guid.Empty.ToString();
            var endpoint = context.HttpContext.Request.Path;
            await next();

            string subject = _natsIntegration.Subject(NATsEventModuleEnum.LOGGER, NATsEventActionEnum.DEBUG, NATsEventStatusEnum.INFO);

            Utils.BackgroundProcessThreadAsync(async Task () =>
            {
                try
                {
                    var data = new
                    {
                        userId,
                        method,
                        endpoint,
                        actionArguments = context.ActionArguments,
                    };

                    var reply = await _natsIntegration.PublishAndGetReply<object, object>(subject, Utils.JsonSerialize(data));
                    _loggerIntegration.LogInformation("Publish NATs Event Reply with Subject : {Subject} | Reply : {Reply}", subject, reply);
                }
                catch (Exception err)
                {
                    _loggerIntegration.LogInformation("Publish NATs Event Error : {Subject}", subject);
                    _loggerIntegration.LogError("Error StackTrace: {StackTrace}", err.StackTrace);
                }
            });
        }
    }
}