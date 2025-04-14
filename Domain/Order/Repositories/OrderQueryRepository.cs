using System.Linq.Expressions;
using DotNetOrderService.Infrastructure.Dtos;
using DotNetOrderService.Infrastructure.Databases;
using DotNetOrderService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetOrderService.Domain.Order.Dtos;

namespace DotNetOrderService.Domain.Order.Repositories
{
    public partial class OrderQueryRepository(
        DotNetOrderServiceDBContext context
    )
    {
        private readonly DotNetOrderServiceDBContext _context = context;

        public async Task<PaginationResult<Models.Order>> Pagination(OrderQueryDto queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Orders
                .Include(data => data.OrderProducts)
                .ThenInclude(data => data.OrderProductDetail)
                .AsNoTracking()
                .AsQueryable();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();
            var count = await Count(query);

            return new PaginationResult<Models.Order>
            {
                Data = data,
                Count = count,
            };
        }

        private static IQueryable<Models.Order> QuerySearch(IQueryable<Models.Order> query, OrderQueryDto queryParams)
        {
            if (!queryParams.Search.IsNullOrEmpty())
            {
                query = query.Where(data =>
                    data.OrderNumber.Contains(queryParams.Search)
                );
            }

            return query;
        }

        private static IQueryable<Models.Order> QueryFilter(IQueryable<Models.Order> query, OrderQueryDto queryParams)
        {
            if (!queryParams.OrderNumber.IsNullOrEmpty())
            {
                query = query.Where(data => data.OrderNumber.Equals(queryParams.OrderNumber));
            }

            return query;
        }

        private static IQueryable<Models.Order> QuerySort(IQueryable<Models.Order> query, OrderQueryDto queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Order, object>>> sortFunctions = new()
            {
                { "order_number", data => data.OrderNumber },
                { "updated_at", data => data.UpdatedAt! },
                { "created_at", data => data.CreatedAt! },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Order, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: {string.Join(", ", sortFunctions.Keys)}");
            }

            query = queryParams.Order == SortOrder.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(IQueryable<Models.Order> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }
    }

    public partial class OrderQueryRepository
    {
        public async Task<Models.Order> FindOneById(Guid id = default)
        {
            return await _context.Orders
                .Where(data => data.Id == id)
                .Include(data => data.OrderProducts)
                .ThenInclude(data => data.OrderProductDetail)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }
    }
}
