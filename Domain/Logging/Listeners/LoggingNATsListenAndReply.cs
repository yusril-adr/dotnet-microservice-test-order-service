using DotNetOrderService.Constants.Logger;
using DotNetOrderService.Domain.Logging.Services;
using DotNetOrderService.Infrastructure.Shareds;
using DotNetOrderService.Infrastructure.Subscriptions;

namespace DotNetOrderService.Domain.Logging.Listeners
{
    public class LoggingNATsListenAndReply(
        ILoggerFactory loggerFactory,
        LoggingService loggingService
    ) : IReplyAction<IDictionary<string, object>, IDictionary<string, object>>
    {

        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
        public readonly LoggingService _loggingService = loggingService;

        public IDictionary<string, object> Reply(IDictionary<string, object> data)
        {
            var jsonData = Utils.JsonSerialize(data);
            
            _logger.LogInformation("<LoggingNATsListenAndReply> : {jsonData}", jsonData);

            return Utils.SuccessResponseFormat();
        }
    }
}
