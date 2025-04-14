using DotNetOrderService.Infrastructure.BackgroundHosted;
using DotNetOrderService.Infrastructure.Integrations.Http;
using DotNetOrderService.Infrastructure.Integrations.NATs;

namespace DotNetOrderService
{
    public partial class Startup
    {
        public void Integrations(IServiceCollection services)
        {
            services.AddScoped<HttpIntegration>();
            services.AddSingleton<NATsIntegration>();
            services.AddSingleton<NATsTask>();
        }
    }
}
