using DotNetService.Commands;
using RuangDeveloper.AspNetCore.Command;

namespace DotNetService
{
    public partial class Startup
    {
        public void Commands(IServiceCollection services)
        {
            services.AddCommands(configure => {
                configure.AddCommand<SeederCommand>();
            });
        }
    }
}
