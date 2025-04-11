using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Helpers;
using DotNetService.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Infrastructure.Seeders
{
  class RolePermissionJson
  {
    [JsonPropertyName("permissionKey")]
    public string PermissionKey { get; set; }

    [JsonPropertyName("roleKey")]
    public string RoleKey { get; set; }
  }

  public class RolePermissionSeeder : ISeeder
  {
    public async Task Seed(DotnetServiceDBContext dbContext, ILogger logger)
    {
      logger.LogInformation("Seeding User Role Permissions...");
      var jsonPath = "SeedersData/RolePermission.json";

      var jsonString = await File.ReadAllTextAsync(jsonPath);
      var datas = JsonSerializer.Deserialize<List<RolePermissionJson>>(jsonString, JsonSerializeSeeder.options);

      if (datas == null || datas.Count == 0)
      {
        logger.LogInformation("No role permissions to seed.");
        return;
      }

      await using var transaction = await dbContext.Database.BeginTransactionAsync();

      try
      {
        var newRolePermissions = new List<RolePermission>();

        foreach (var data in datas)
        {
          var permission = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Key == data.PermissionKey);
          var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Key == data.RoleKey);

          if (permission == null || role == null)
          {
            logger.LogWarning("Permission or Role not found for PermissionKey: {PermissionKey}, RoleKey: {RoleKey}", data.PermissionKey, data.RoleKey);
            continue;
          }

          var newRolePermission = new RolePermission
          {
            Id = Guid.NewGuid(),
            PermissionId = permission.Id,
            RoleId = role.Id,
            CreatedAt = DateTime.Now
          };
          newRolePermissions.Add(newRolePermission);
        }

        if (newRolePermissions.Count != 0)
        {
          await dbContext.RolePermissions.AddRangeAsync(newRolePermissions);
          await dbContext.SaveChangesAsync();
          await transaction.CommitAsync();
        }
        else
        {
          logger.LogInformation("No valid role permissions to seed.");
        }
      }
      catch (Exception e)
      {
        logger.LogError(e, "Error while seeding Role Permissions");
        await transaction.RollbackAsync();
      }

      logger.LogInformation("Seeding Role Permissions complete");
    }
  }
}