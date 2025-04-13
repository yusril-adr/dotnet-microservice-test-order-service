using DotnetOrderService.Constants.Order;

namespace DotnetOrderService.Models
{
    public class Order : BaseModel
    {
        public string OrderNumber { get; set; }

        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
