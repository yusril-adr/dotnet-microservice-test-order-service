using System.Linq.Expressions;
using DotNetService.Domain.User.Dtos;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotNetService.Domain.User.Repositories
{
    public partial class UserQueryRepository(
        DotnetServiceDBContext context
        )
    {
        private readonly DotnetServiceDBContext _context = context;

        public async Task<PaginationResult<Models.User>> Pagination(UserQueryDto queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Users
            .Include(data => data.UserRoles)
            .ThenInclude(data => data.Role)
            .AsNoTracking()
            .AsQueryable();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();
            var count = await Count(query);

            return new PaginationResult<Models.User>
            {
                Data = data,
                Count = count,
            };
        }

        private static IQueryable<Models.User> QuerySearch(IQueryable<Models.User> query, UserQueryDto queryParams)
        {
            if (!queryParams.Search.IsNullOrEmpty())
            {
                query = query.Where(data =>
                    data.Name.Contains(queryParams.Search) ||
                    data.Email.Contains(queryParams.Search)
                );
            }

            return query;
        }

        private static IQueryable<Models.User> QueryFilter(IQueryable<Models.User> query, UserQueryDto queryParams)
        {
            if (!queryParams.Email.IsNullOrEmpty())
            {
                query = query.Where(data => data.Email.Equals(queryParams.Email));
            }

            return query;
        }

        private static IQueryable<Models.User> QuerySort(IQueryable<Models.User> query, UserQueryDto queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.User, object>>> sortFunctions = new()
            {
                { "name", data => data.Name },
                { "email", data => data.Email },
                { "updated_at", data => data.UpdatedAt! },
                { "created_at", data => data.CreatedAt! },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.User, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: {string.Join(", ", sortFunctions.Keys)}");
            }

            query = queryParams.Order == SortOrder.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(IQueryable<Models.User> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }
    }

    public partial class UserQueryRepository
    {

        public async Task<Models.User> FindOneById(Guid id = default)
        {
            return await _context.Users
                .Where(data => data.Id == id)
                .Include(data => data.UserRoles)
                .ThenInclude(data => data.Role)
                .ThenInclude(data => data.RolePermissions)
                .ThenInclude(data => data.Permission)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public async Task<Models.User> FindOneByEmail(string email)
        {
            return await _context.Users.Where(data => data.Email == email).SingleOrDefaultAsync();
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await _context.Users.AnyAsync(data => data.Email == email);
        }

        public async Task<bool> IsEmailExistsExceptId(string email, Guid id)
        {
            return await _context.Users.AnyAsync(data => data.Email == email && data.Id != id);
        }
    }
}
