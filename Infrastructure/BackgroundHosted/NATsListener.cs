using DotNetService.Constants.Logger;

namespace DotNetService.Infrastructure.BackgroundHosted
{
    public class NATsListener(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory
    ) : IHostedService, IDisposable
    {

        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.INTEGRATION);
        public readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public void Dispose()
        {
            // TODO: Dispose
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _logger.LogInformation("NATs Subscription Hosted Service running listen.");
                scope.ServiceProvider.GetRequiredService<NATsTask>().Listen();
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _logger.LogInformation("NATs Subscription Hosted Service running reply.");
                scope.ServiceProvider.GetRequiredService<NATsTask>().ListenAndReply();
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _logger.LogInformation("NATs Subscription Hosted Service running reply.");
                scope.ServiceProvider.GetRequiredService<NATsTask>().ConsumeJetStream();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NATs Subscription Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
