using DotnetOrderService.Constants.Logger;
using DotnetOrderService.Domain.Logging.Services;
using DotnetOrderService.Infrastructure.Shareds;
using DotnetOrderService.Infrastructure.Subscriptions;

namespace DotnetOrderService.Domain.Logging.Listeners
{
    public class LoggingNATsListener(
        ILoggerFactory loggerFactory,
        LoggingService loggingService
    ) : ISubscriptionAction<IDictionary<string, object>>
    {
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
        public readonly LoggingService _loggingService = loggingService;

        public void Handle(IDictionary<string, object> data)
        {
            // EXAMPLE: Do operation event
            var jsonData = Utils.JsonSerialize(data);
            _logger.LogInformation(jsonData);
        }
    }
}
