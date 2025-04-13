using DotnetOrderService.Domain.Order.Repositories;

namespace DotnetOrderService
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
