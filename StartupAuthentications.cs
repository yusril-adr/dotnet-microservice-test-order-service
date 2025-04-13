using DotnetOrderService.Infrastructure.Databases;

namespace DotnetOrderService
{
    public partial class Startup
    {
        public void Authentications(IServiceCollection services)
        {
            services.AddSingleton<LocalStorageDatabase>();
        }
    }
}
