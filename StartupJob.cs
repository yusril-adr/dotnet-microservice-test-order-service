using DotNetService.Infrastructure.Jobs;

namespace DotNetService
{
    public partial class Startup
    {
        public void Jobs(IServiceCollection services)
        {
            services.AddScoped<NotificationHouseKeepingJob>();
        }
    }
}
