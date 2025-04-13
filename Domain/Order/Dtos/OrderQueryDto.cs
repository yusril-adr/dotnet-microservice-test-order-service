using DotnetOrderService.Infrastructure.Dtos;

namespace DotnetOrderService.Domain.Order.Dtos
{
    public class OrderQueryDto : QueryDto
    {
        public string OrderNumber { get; set; }
    }
}