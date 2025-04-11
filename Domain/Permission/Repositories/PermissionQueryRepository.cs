using System.Linq.Expressions;
using DotNetService.Domain.Permission.Dtos;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotNetService.Domain.Permission.Repositories
{
    public class PermissionQueryRepository(
        DotnetServiceDBContext context
        )
    {
        private readonly DotnetServiceDBContext _context = context;

        public async Task<PaginationResult<Models.Permission>> Pagination(PermissionQueryDto queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Permissions
                .Include(data => data.RolePermissions)
                .AsQueryable()
                .AsNoTracking();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();
            var count = await Count(query);

            return new PaginationResult<Models.Permission>
            {
                Data = data,
                Count = count,
            };
        }

        private static IQueryable<Models.Permission> QuerySearch(IQueryable<Models.Permission> query, PermissionQueryDto queryParams)
        {
            if (!queryParams.Search.IsNullOrEmpty())
            {
                query = query.Where(data =>
                    data.Name.Contains(queryParams.Search));
            }

            return query;
        }

        private static IQueryable<Models.Permission> QueryFilter(IQueryable<Models.Permission> query, PermissionQueryDto queryParams)
        {
            if (!queryParams.Name.IsNullOrEmpty())
            {
                query = query.Where(data => data.Name.Equals(queryParams.Name));
            }

            return query;
        }

        private static IQueryable<Models.Permission> QuerySort(IQueryable<Models.Permission> query, PermissionQueryDto queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Permission, object>>> sortFunctions = new()
            {
                { "name", data => data.Name },
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Permission, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrder.Asc
               ? query.OrderBy(value).AsQueryable()
               : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(IQueryable<Models.Permission> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }

        public async Task<Models.Permission> FindOneById(Guid id = default)
        {
            return await _context.Permissions
                .Where(data => data.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<List<string>> FindPermissionByUserId(Guid userId)
        {
            var permissions = await (
                from ur in _context.UserRoles
                join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                join p in _context.Permissions on rp.PermissionId equals p.Id
                where ur.UserId == userId
                select p.Key
            )
            .Distinct()
            .ToListAsync();

            return permissions.Count == 0 ? [] : permissions;
        }

        public async Task<List<Models.Permission>> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Permission> permissions;
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (!search.IsNullOrEmpty())
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }
            permissions = await permissionQuery.Skip(skip).Take(perPage).ToListAsync();

            return permissions;
        }

        public async Task<int> CountAll(string search)
        {
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (!search.IsNullOrEmpty())
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }

            return await permissionQuery.CountAsync();
        }

        public async Task<bool> IsExistByKey(string key)
        {
            return await _context.Permissions.Where(permission => permission.Key == key).AnyAsync();
        }
    }
}
