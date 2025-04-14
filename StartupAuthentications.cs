using DotNetOrderService.Infrastructure.Databases;

namespace DotNetOrderService
{
    public partial class Startup
    {
        public void Authentications(IServiceCollection services)
        {
            services.AddSingleton<LocalStorageDatabase>();
        }
    }
}
