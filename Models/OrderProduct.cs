using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetOrderService.Models
{
    public class OrderProduct : BaseModel
    {
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        public Guid OrderProductDetailId { get; set; }

        [ForeignKey(nameof(OrderProductDetailId))]
        public virtual OrderProductDetail OrderProductDetail { get; set; }

        public Guid ProductId { get; set; }
        
        public decimal ProductItemPrice { get; set; }
        
        public int ProductQuantity { get; set; }
    }
}
