using DotnetOrderService.Infrastructure.BackgroundHosted;
using DotnetOrderService.Infrastructure.Integrations.Http;
using DotnetOrderService.Infrastructure.Integrations.NATs;

namespace DotnetOrderService
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
