using DotNetOrderService.Domain.Order.Repositories;

namespace DotNetOrderService
{
    public partial class Startup
    {
        public void Repositories(IServiceCollection services)
        {
            services.AddScoped<OrderQueryRepository>();
            services.AddScoped<OrderStoreRepository>();
        }
    }
}
