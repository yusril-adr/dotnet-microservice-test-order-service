using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Helpers;
using DotNetService.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Infrastructure.Seeders
{
  class UserRoleJson
  {
    public string Email { get; set; }

    [JsonPropertyName("roleKey")]
    public string RoleKey { get; set; }
  }

  public class UserRoleSeeder : ISeeder
  {
    public async Task Seed(DotnetServiceDBContext dbContext, ILogger logger)
    {
      logger.LogInformation("Seeding User Roles...");
      var jsonPath = "SeedersData/UserRole.json";

      var jsonString = await File.ReadAllTextAsync(jsonPath);
      var userRoles = JsonSerializer.Deserialize<List<UserRoleJson>>(jsonString, JsonSerializeSeeder.options);

      if (userRoles == null || userRoles.Count == 0)
      {
        logger.LogInformation("No user roles to seed.");
        return;
      }

      await using var transaction = await dbContext.Database.BeginTransactionAsync();

      try
      {
        var newUserRoles = new List<UserRole>();

        foreach (var userRole in userRoles)
        {
          var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == userRole.Email);
          var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Key == userRole.RoleKey);

          if (user == null || role == null)
          {
            logger.LogWarning("User or Role not found for Email: {Email}, RoleKey: {RoleKey}", userRole.Email, userRole.RoleKey);
            continue;
          }

          var newUserRole = new UserRole
          {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
            CreatedAt = DateTime.Now
          };
          newUserRoles.Add(newUserRole);
        }

        if (newUserRoles.Count != 0)
        {
          await dbContext.UserRoles.AddRangeAsync(newUserRoles);
          await dbContext.SaveChangesAsync();
          await transaction.CommitAsync();
        }
        else
        {
          logger.LogInformation("No valid user roles to seed.");
        }
      }
      catch (Exception e)
      {
        logger.LogError(e, "Error while seeding User Roles");
        await transaction.RollbackAsync();
      }

      logger.LogInformation("Seeding User Roles complete");
    }
  }
}