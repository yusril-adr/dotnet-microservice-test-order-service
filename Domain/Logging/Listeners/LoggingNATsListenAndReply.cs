using DotNetService.Constants.Logger;
using DotNetService.Domain.Logging.Services;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Subscriptions;

namespace DotNetService.Domain.Logging.Listeners
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
