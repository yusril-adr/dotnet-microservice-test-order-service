namespace DotNetOrderService.Domain.Order.Dtos {
    
    public class OrderCheckoutReplyDto
    {
        public List<OrderCheckoutStockReplyDto> ValidStocks { get; set; } = [];
        public List<OrderCheckoutStockReplyDto> InsufficientStock { get; set; } = [];
        public List<Guid> StockNotFoundProductIds { get; set; } = [];
    }

    public class OrderCheckoutStockReplyDto
    {
        public Guid ProductId { get; set; }
        public int Stock { get; set; }
    }
}