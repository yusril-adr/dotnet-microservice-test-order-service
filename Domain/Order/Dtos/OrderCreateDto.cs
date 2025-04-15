using DotNetOrderService.Infrastructure.Shareds;
using System.ComponentModel.DataAnnotations;

namespace DotNetOrderService.Domain.Order.Dtos
{
    public class OrderCreateDto
    {
        [Required]
        [MinLength(1)]
        public List<OrderProductCreateDto> OrderProducts { get; set; } = new List<OrderProductCreateDto>();

        public static Models.Order Assign(OrderCreateDto orderCreateDto) {
            return new Models.Order {
                OrderNumber = Utils.RandStr(15),
                TotalPrice = orderCreateDto.OrderProducts.Sum(op => op.ProductItemPrice * op.ProductQuantity)
            };
        }
    }

    public class OrderProductCreateDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public decimal ProductItemPrice { get; set; }

        [Required]
        public int ProductQuantity { get; set; }
    }
}
