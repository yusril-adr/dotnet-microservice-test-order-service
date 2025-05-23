using DotNetOrderService.Constants.Logger;
using DotNetOrderService.Domain.Logging.Services;
using DotNetOrderService.Infrastructure.Shareds;
using DotNetOrderService.Infrastructure.Subscriptions;

namespace DotNetOrderService.Domain.Logging.Listeners
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
