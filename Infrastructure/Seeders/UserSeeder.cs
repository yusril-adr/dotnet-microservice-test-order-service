using System.Text.Json;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Helpers;
using DotNetService.Models;
using BC = BCrypt.Net.BCrypt;

namespace DotNetService.Infrastructure.Seeders
{
  public class UserSeeder : ISeeder
  {
    public async Task Seed(DotnetServiceDBContext dbContext, ILogger logger)
    {
      logger.LogInformation("Seeding Users...");
      var jsonPath = "SeedersData/User.json";

      var jsonString = await File.ReadAllTextAsync(jsonPath);

      var datas = JsonSerializer.Deserialize<List<User>>(jsonString, JsonSerializeSeeder.options);
      var formatedData = new List<User>();

      try
      {
        await dbContext.Database.BeginTransactionAsync();

        foreach (var data in datas)
        {
          data.Id = Guid.NewGuid();

          data.Password = BC.HashPassword(data.Password);

          data.CreatedAt = DateTime.Now;
          formatedData.Add(data);
        }
        await dbContext.Users.AddRangeAsync(formatedData);
        await dbContext.SaveChangesAsync();
        await dbContext.Database.CommitTransactionAsync();
      }
      catch (Exception e)
      {
        logger.LogError(e, "Error while seeding Users");
        await dbContext.Database.RollbackTransactionAsync();
      }

      logger.LogInformation("Seeding Users complete");
    }
  }
}