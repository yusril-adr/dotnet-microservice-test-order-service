using DotNetService.Infrastructure.Databases;

namespace DotNetService.Infrastructure.Seeders
{
  public interface ISeeder
  {
    Task Seed(DotnetServiceDBContext dbContext, ILogger logger);
  }
}
