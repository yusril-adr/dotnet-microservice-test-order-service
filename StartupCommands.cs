using RuangDeveloper.AspNetCore.Command;

namespace DotnetOrderService
{
    public partial class Startup
    {
        public void Commands(IServiceCollection services)
        {
            services.AddCommands(configure => {
            });
        }
    }
}
