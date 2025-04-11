using DotNetService.Domain.Auth.Util;
using DotNetService.Infrastructure.Databases;

namespace DotNetService
{
    public partial class Startup
    {
        public void Authentications(IServiceCollection services)
        {
            services.AddSingleton<LocalStorageDatabase>();
            services.AddSingleton<AuthUtil>();
        }
    }
}
