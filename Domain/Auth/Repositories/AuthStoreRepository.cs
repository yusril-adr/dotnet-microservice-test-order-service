using DotNetService.Infrastructure.Databases;

namespace DotNetService.Domain.Auth.Repositories
{
    public class AuthStoreRepository(
        DotnetServiceDBContext context
        )
    {
        private readonly DotnetServiceDBContext _context = context;

        public async Task Create(Models.User data)
        {
            _context.Users.Add(data);
            await _context.SaveChangesAsync();
        }
    }
}