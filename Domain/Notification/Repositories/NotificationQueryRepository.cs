using System.Linq.Expressions;
using DotNetService.Domain.Notification.Dtos;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.Notification.Repositories
{
    public class NotificationQueryRepository(
        DotnetServiceDBContext context
        )
    {
        private readonly DotnetServiceDBContext _context = context;

        public async Task<PaginationResult<Models.Notification>> Pagination(NotificationQueryDto queryParams, Guid userId)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Notifications
                .AsQueryable()
                .AsNoTracking();

            query = QueryFilter(query, userId);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();
            var count = await Count(query);

            return new PaginationResult<Models.Notification>
            {
                Data = data,
                Count = count,
            };
        }

        private static IQueryable<Models.Notification> QueryFilter(IQueryable<Models.Notification> query, Guid userId)
        {
            query = query.Where(data => data.UserId == userId);
            return query;
        }

        private static IQueryable<Models.Notification> QuerySort(IQueryable<Models.Notification> query, NotificationQueryDto queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Notification, object>>> sortFunctions = new()
            {
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Notification, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrder.Asc
               ? query.OrderBy(value).AsQueryable()
               : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(IQueryable<Models.Notification> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }

        public async Task<Models.Notification> FindOneByIdAndUserId(Guid id, Guid userId)
        {
            return await _context.Notifications
                .Where(data => data.Id == id && data.UserId == userId)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> HasUnreadNotificationByUserId(Guid userId)
        {
            return await _context.Notifications
                .Where(data => data.UserId == userId && !data.IsRead)
                .AnyAsync();
        }
    }
}