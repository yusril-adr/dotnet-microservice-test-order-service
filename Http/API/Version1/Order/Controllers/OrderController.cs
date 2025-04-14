using DotNetOrderService.Domain.Order.Dtos;
using DotNetOrderService.Domain.Order.Services;
using DotNetOrderService.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotNetOrderService.Http.API.Version1.Order.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]

    public class OrderController(
        OrderService OrderService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<ApiResponse> Index([FromQuery] OrderQueryDto query)
        {
            var paginationResult = await OrderService.Index(query);
            return new ApiResponsePagination<OrderResultDto>(HttpStatusCode.OK, paginationResult);
        }

        [HttpGet("{orderId}")]
        public async Task<ApiResponse> Detail(Guid orderId)
        {
            var result = await OrderService.Detail(orderId);
            return new ApiResponseData<OrderResultDto>(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public async Task<ApiResponse> Create(OrderCreateDto dataCreate) {
            var result = await OrderService.Create(dataCreate);
            return new ApiResponseData<OrderResultDto>(HttpStatusCode.Created, result);
        }
    }
}