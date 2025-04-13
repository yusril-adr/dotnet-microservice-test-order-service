using DotnetOrderService.Domain.Logging.Listeners;

namespace DotnetOrderService
{
    public partial class Startup
    {
        public void Listeners(IServiceCollection services)
        {
            services.AddScoped<LoggingNATsListener>();
            services.AddScoped<LoggingNATsListenAndReply>();
        }
    }
}
