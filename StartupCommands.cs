using RuangDeveloper.AspNetCore.Command;

namespace DotNetOrderService
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
