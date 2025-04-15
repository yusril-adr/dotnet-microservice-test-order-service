using DotNetOrderService.Infrastructure.Databases;
using DotNetOrderService.Infrastructure.Exceptions;
using DotNetOrderService.Infrastructure.Shareds;
using DotNetOrderService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetOrderService.Domain.Order.Repositories
{
    public class OrderStoreRepository(
        DotNetOrderServiceDBContext context
    )
    {
        private readonly DotNetOrderServiceDBContext _context = context;

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
                await _context.SaveChangesAsync(); // to made new order have id

                if (productDetails?.Count > 0) {
                    var orderProductDetails = productDetails.Select(product => {
                        return new OrderProductDetail {
                            OrderId = order.Id,
                            ProductId = product.Item1,
                            Name = product.Item2,
                        };
                    }).ToList();
                    await _context.OrderProductDetails.AddRangeAsync(orderProductDetails);

                    if (orderProducts?.Count > 0)
                    {
                        var validOrderProducts = new List<OrderProduct>();
                        orderProducts.ForEach((orderProduct) => {
                            orderProduct.Order = order;

                            var orderProductDetail = orderProductDetails.FirstOrDefault(
                                orderProductDetail => orderProductDetail.ProductId == orderProduct.ProductId
                            );

                            if (orderProductDetail is null) {
                                return;
                            }

                            orderProduct.OrderProductDetail = orderProductDetail;
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
    
        public async Task Update(
            Models.Order order
        ) {
            context.Orders.Update(order);

            await context.SaveChangesAsync();
        }
    }
}