namespace DotNetOrderService.Domain.Order.Dtos {
    public class OrderCheckoutPublishDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public OrderCheckoutPublishDto(Models.OrderProduct orderProduct) {
            ProductId = orderProduct.ProductId;
            Quantity = orderProduct.ProductQuantity;
        }

        public static List<OrderCheckoutPublishDto> MapRepo(List<Models.OrderProduct> orderProducts) {
            return orderProducts.Select(orderProduct => new OrderCheckoutPublishDto(orderProduct)).ToList();
        }
    }
}