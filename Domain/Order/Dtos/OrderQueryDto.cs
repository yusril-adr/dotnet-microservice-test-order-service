using DotNetOrderService.Infrastructure.Dtos;

namespace DotNetOrderService.Domain.Order.Dtos
{
    public class OrderQueryDto : QueryDto
    {
        public string OrderNumber { get; set; }
    }
}