using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Exceptions;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.Role.Repositories
{
    public class RoleStoreRepository(
        DotnetServiceDBContext context
        )
    {
        private readonly DotnetServiceDBContext _context = context;

        public async Task Create(Models.Role role, List<Guid> permissionIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newRole = await _context.Roles.AddAsync(new Models.Role
                {
                    Id = Guid.NewGuid(),
                    Name = role.Name,
                    Key = role.Key
                });
                var createdRole = newRole.Entity;

                if (permissionIds?.Count > 0)
                {
                    var rolePermissions = new List<Models.RolePermission>();
                    foreach (var permissionId in permissionIds)
                    {
                        var rolePermission = new Models.RolePermission
                        {
                            RoleId = createdRole.Id,
                            PermissionId = permissionId
                        };
                        rolePermissions.Add(rolePermission);
                    }
                    await _context.RolePermissions.AddRangeAsync(rolePermissions);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task Update(Guid id, Models.Role roleRepository, List<Guid> permissionIds)
        {
            // Start a transaction 
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update the role
                Models.Role data = new() { Id = id };
                _context.Roles.Attach(data);
                _context.Roles.Update(roleRepository);

                // Update the role permissions
                if (permissionIds?.Count > 0)
                {
                    // Remove all role permissions
                    _context.RolePermissions.RemoveRange(_context.RolePermissions.Where(x => x.RoleId == id));

                    // Add new role permissions
                    var rolePermissions = new List<Models.RolePermission>();
                    foreach (var permissionId in permissionIds)
                    {
                        var rolePermission = new Models.RolePermission
                        {
                            RoleId = id,
                            PermissionId = permissionId
                        };
                        rolePermissions.Add(rolePermission);
                    }
                    await _context.RolePermissions.AddRangeAsync(rolePermissions);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                await _context.Database.RollbackTransactionAsync();
                throw new UnprocessableEntityException("No data was updated.");
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.Role data = new() { Id = id };
                _context.Roles.Attach(data);
                _context.Roles.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }
    }
}