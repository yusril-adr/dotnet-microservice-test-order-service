using DotnetOrderService.Infrastructure.Dtos;

namespace DotnetOrderService.Domain.Order.Dtos
{
    public class OrderResultDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public EnumDto Status { get; set; }

        public List<OrderProductResultDto> OrderProducts { get; set; }

        public OrderResultDto(Models.Order order)
        {
            Id = order.Id;
            OrderNumber = order.OrderNumber;
            TotalPrice = order.TotalPrice;
            Status = new EnumDto((int) order.OrderStatus, order.OrderStatus.ToString());
        }

        public static List<OrderResultDto> MapRepo(List<Models.Order> data)
        {
            return data?.Select(data => new OrderResultDto(data)).ToList();
        }
    }

    public class OrderProductResultDto {
        public Guid Id { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public OrderProductDetailResultDto Product { get; set; }

        public OrderProductResultDto(Models.OrderProduct orderProduct) {
            Id = orderProduct.Id;
            ItemPrice = orderProduct.ProductItemPrice;
            Quantity = orderProduct.ProductQuantity;

            if (orderProduct.OrderProductDetail is not null) {
                Product = new OrderProductDetailResultDto(orderProduct.OrderProductDetail);
            }
        }
    }

    public class OrderProductDetailResultDto {
        public Guid ProductId { get; set; }
        public string Name { get; set; }

        public OrderProductDetailResultDto(Models.OrderProductDetail productDetail) {
            ProductId = productDetail.ProductId;
            Name = productDetail.Name;
        }
    }
}