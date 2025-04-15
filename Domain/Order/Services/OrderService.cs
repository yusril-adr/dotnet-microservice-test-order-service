using DotNetOrderService.Domain.Order.Repositories;
using DotNetOrderService.Infrastructure.Exceptions;
using DotNetOrderService.Domain.Order.Dtos;
using DotNetOrderService.Infrastructure.Dtos;
using DotNetOrderService.Domain.Order.Messages;
using DotNetOrderService.Infrastructure.Shareds;
using DotNetOrderService.Constants.Event;
using DotNetOrderService.Infrastructure.Integrations.NATs;
using document_generator.Infrastructure.Helpers;
using DotNetOrderService.Constants.Order;

namespace DotNetOrderService.Domain.Order.Services
{
    public class OrderService(
        NATsIntegration natsIntegration,
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

            var productDetailsReplyRaw = await natsIntegration.PublishAndGetReply<object, object>(
                natsIntegration.Subject(
                    NATsEventModuleEnum.PRODUCT,
                    NATsEventActionEnum.GET_BY_IDS,
                    NATsEventStatusEnum.REQUEST
                ),
                Utils.JsonSerialize(new {
                    data = productIds
                })
            );

            var productDetailsReply = Utils.ParseFromNATSReply<ResponseFormat<List<OrderProductDetailReplyDto>>>(productDetailsReplyRaw);

            var productDetails = productDetailsReply.Data.Select(productDetail => (productDetail.Id, productDetail.Name)).ToList();
            await orderStoreRepository.Create(order, orderProducts, productDetails);

            var checkoutProductStockReplyRaw = await natsIntegration.PublishAndGetReply<object, object>(
                natsIntegration.Subject(
                    NATsEventModuleEnum.PRODUCT,
                    NATsEventActionEnum.UPDATE,
                    NATsEventStatusEnum.REQUEST
                ),
                Utils.JsonSerialize(new {
                    data = OrderCheckoutPublishDto.MapRepo(orderProducts)
                })
            );

            var checkoutProductStockReply = Utils.ParseFromNATSReply<ResponseFormat<OrderCheckoutReplyDto>>(checkoutProductStockReplyRaw);

            order.OrderStatus = checkoutProductStockReply.Success ? OrderStatus.Confirmed : OrderStatus.Rejected;
            await orderStoreRepository.Update(order);

            if (!checkoutProductStockReply.Success) {
                throw new UnprocessableEntityException(checkoutProductStockReply.Message);
            }

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