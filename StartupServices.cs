using DotNetOrderService.Domain.File.Services;
using DotNetOrderService.Domain.Logging.Services;
using DotNetOrderService.Domain.Order.Services;
using DotNetOrderService.Infrastructure.Shareds;

namespace DotNetOrderService
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
