using DotNetOrderService.Domain.Order.Repositories;
using DotNetOrderService.Infrastructure.Exceptions;
using DotNetOrderService.Domain.Order.Dtos;
using DotNetOrderService.Infrastructure.Dtos;
using DotNetOrderService.Domain.Order.Messages;
using DotNetOrderService.Infrastructure.Shareds;

namespace DotNetOrderService.Domain.Order.Services
{
    public class OrderService(
        OrderQueryRepository orderQueryRepository,
        OrderStoreRepository orderStoreRepository
    )
    {
        public async Task<PaginationModel<OrderResultDto>> Index(OrderQueryDto query)
        {
            var result = await orderQueryRepository.Pagination(query);
            var formatedResult = OrderResultDto.MapRepo(result.Data);
            var paginate = PaginationModel<OrderResultDto>.Parse(formatedResult, result.Count, query);
            return paginate;
        }


        public async Task<OrderResultDto> Create(OrderCreateDto dataCreate)
        {
            var order = OrderCreateDto.Assign(dataCreate);
            var orderProducts = dataCreate.OrderProducts
                .Select(
                    orderProduct => 
                        new Models.OrderProduct {
                            ProductId = orderProduct.ProductId,
                            ProductItemPrice = orderProduct.ProductItemPrice,
                            ProductQuantity = orderProduct.ProductQuantity,
                        }
                )
                .ToList();
            
            var productIds = orderProducts.Select(op => op.ProductId).Distinct();

            // TODO: Get productDetail from other Service
            // Using dummy for temporary
            var productDetails = productIds.Select(id => {
                var dummyName = Utils.RandStr(5);

                return (id, dummyName);
            }).ToList();

            await orderStoreRepository.Create(order, orderProducts, productDetails);

            return await this.Detail(order.Id);
        }

        public async Task<OrderResultDto> Detail(Guid id)
        {
            var order = await orderQueryRepository.FindOneById(id);
            if (order == null)
            {
                throw new DataNotFoundException(OrderErrorMessage.OrderNotFound);
            }

            return new OrderResultDto(order);
        }
    }
}