using DotnetOrderService.Domain.File.Services;
using DotnetOrderService.Domain.Logging.Services;
using DotnetOrderService.Domain.Order.Services;
using DotnetOrderService.Infrastructure.Shareds;

namespace DotnetOrderService
{
    public partial class Startup
    {
        public void Services(IServiceCollection services)
        {
            services.AddScoped<OrderService>();
            
            services.AddScoped<LoggingService>();
            services.AddScoped<StorageService>();
            services.AddScoped<FileService>();
        }
    }
}
