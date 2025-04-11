using DotNetService.Infrastructure.BackgroundHosted;
using DotNetService.Infrastructure.Integrations.Http;
using DotNetService.Infrastructure.Integrations.NATs;

namespace DotNetService
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
