using DotnetOrderService.Infrastructure.Databases;
using DotnetOrderService.Infrastructure.Exceptions;
using DotnetOrderService.Models;
using Microsoft.EntityFrameworkCore;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotnetOrderService.Domain.Order.Repositories
{
    public class OrderStoreRepository(
        DotnetOrderServiceDBContext context
    )
    {
        private readonly DotnetOrderServiceDBContext _context = context;

        public async Task Create(
            Models.Order order,
            List<OrderProduct> orderProducts,
            List<(Guid, string)> productDetails // productModels from other services
        )
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                order.OrderStatus = Constants.Order.OrderStatus.Pending;
                await _context.Orders.AddAsync(order);

                if (productDetails?.Count > 0) {
                    var orderProductDetails = productDetails.Select(product => {
                        return new OrderProductDetail {
                            ProductId = product.Item1,
                            Name = product.Item2,
                        };
                    }).ToList();
                    await _context.OrderProductDetails.AddRangeAsync(orderProductDetails);

                    if (orderProducts?.Count > 0)
                    {
                        var validOrderProducts = new List<OrderProduct>();
                        orderProducts.ForEach((orderProduct) => {
                            orderProduct.OrderId = order.Id;

                            var orderProductDetail = orderProductDetails.FirstOrDefault(
                                orderProductDetail => orderProductDetail.ProductId == orderProduct.ProductId
                            );

                            if (orderProductDetail is null) {
                                return;
                            }

                            orderProduct.OrderProductDetailId = orderProductDetail.Id;
                            validOrderProducts.Add(orderProduct);
                        });
                        await _context.OrderProducts.AddRangeAsync(orderProducts);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }
    }
}