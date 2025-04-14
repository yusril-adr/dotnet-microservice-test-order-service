using DotNetOrderService.Domain.Logging.Listeners;

namespace DotNetOrderService
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
