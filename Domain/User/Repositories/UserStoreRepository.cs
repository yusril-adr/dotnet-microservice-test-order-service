using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.User.Repositories
{
    public class UserStoreRepository(
        DotnetServiceDBContext context
    )
    {
        private readonly DotnetServiceDBContext _context = context;

        public async Task Create(Models.User data, List<Guid> roleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var newUser = await _context.Users.AddAsync(new Models.User
                {
                    Id = Guid.NewGuid(),
                    Name = data.Name,
                    Email = data.Email,
                    Password = data.Password
                });
                var createdUser = newUser.Entity;

                if (roleIds?.Count > 0)
                {
                    var userRoles = new List<Models.UserRole>();
                    foreach (var roleId in roleIds)
                    {
                        var userRole = new Models.UserRole
                        {
                            UserId = createdUser.Id,
                            RoleId = roleId
                        };
                        userRoles.Add(userRole);
                    }
                    await _context.UserRoles.AddRangeAsync(userRoles);
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

        public async Task Update(Guid id, Models.User newData, List<Guid> roleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Update the user
                Models.User data = new() { Id = id };
                _context.Users.Attach(data);
                _context.Users.Update(newData);

                // Update the user roles
                if (roleIds?.Count > 0)
                {
                    // Remove existing user roles
                    _context.UserRoles.RemoveRange(_context.UserRoles.Where(userRole => userRole.UserId == id));

                    // Add new user roles
                    var userRoles = new List<Models.UserRole>();
                    foreach (var roleId in roleIds)
                    {
                        var userRole = new Models.UserRole
                        {
                            UserId = id,
                            RoleId = roleId
                        };
                        userRoles.Add(userRole);
                    }
                    await _context.UserRoles.AddRangeAsync(userRoles);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
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
                Models.User data = new() { Id = id };
                _context.Users.Attach(data);
                _context.Users.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }
    }
}