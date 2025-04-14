using System.ComponentModel.DataAnnotations;

namespace DotNetOrderService.Domain.Order.Dtos
{
    public class OrderCreateDto
    {
        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderProductCreateDto> OrderProducts { get; set; } = new List<OrderProductCreateDto>();

        public static Models.Order Assign(OrderCreateDto orderCreateDto) {
            return new Models.Order {
                TotalPrice = orderCreateDto.TotalPrice
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
