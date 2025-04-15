namespace DotNetOrderService.Domain.Order.Dtos {
    
    public class OrderProductDetailReplyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? TotalStock { get; set; } = null;
        public List<OrderProductStockReplyDto> Stocks { get; set; } = null;
    }

    public class OrderProductStockReplyDto {
        public Guid Id { get; set; }
        public Guid ProductStock { get; set; }
        public int Stock { get; set; }
    }
}