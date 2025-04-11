
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Exceptions;
using DotNetService.Infrastructure.Seeders;
using RuangDeveloper.AspNetCore.Command;

namespace DotNetService.Commands
{
    public class SeederCommand(
        IServiceProvider serviceProvider
    ) : ICommand
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public string Name => "seed";

        public string Description => "Seed the database with initial data";

        public void Execute(string[] args)
        {
        }

        public async Task ExecuteAsync(string[] args)
        {
            var fileNames = args.ToList();
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DotnetServiceDBContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeederCommand>>();

            Console.WriteLine("-------------------------- Seed Started -------------------------");

            if (fileNames.Count > 0)
            {
                for (var i = 0; i < fileNames.Count; i++)
                {
                    /* -------------------------- Insert seed data here ------------------------- */
                    var type = Type.GetType("DotNetService.Infrastructure.Seeders." + fileNames[i]);
                    logger.LogInformation("Seeding: {SeederName}", fileNames[i]);
                    if (type != null)
                    {
                        if (Activator.CreateInstance(type) is ISeeder seederType)
                        {
                            await seederType.Seed(dbContext, logger);
                        }
                    }
                    else
                    {
                        throw new DataNotFoundException($"seeder of {fileNames[i]} not found");
                    }
                }
            }
            // When it doesnt have any args, seed all data accordingly
            else
            {
                /* -------------------------- Insert seed data here ------------------------- */
                /* ----------------------- Be careful about the sequences ------------------- */
                await new UserSeeder().Seed(dbContext, logger);
                await new RoleSeeder().Seed(dbContext, logger);
                await new PermissionSeeder().Seed(dbContext, logger);
                await new RolePermissionSeeder().Seed(dbContext, logger);
                await new UserRoleSeeder().Seed(dbContext, logger);
            }

            logger.LogInformation("-------------------------- Seed Finish --------------------------");
        }
    }
}
