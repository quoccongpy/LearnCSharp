using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO model)
        {
            var orderId = await _orderService.CreateAsync(model);
            return Ok(new { orderId });
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrderPaging(string? keyword, string? status, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _orderService.GetAllOrderPagingAsync(keyword, status, pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<OrderDTO>> GetByIdOrder(int id)
        {
            var data = await _orderService.GetOrderByIdAsync(id);
            return Ok(data);
        }

        [HttpGet("get-order-by-user")]
        [Authorize]
        public async Task<ActionResult<PagedResult<OrderDTO>>> GetOrdersByUser(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _orderService.GetOrdersByUserAsyncPagingAsync(keyword, pageIndex, pageSize);
            return Ok(data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusDTO model)
        {
            await _orderService.UpdateStatusAsync(id, model.Status);
            return Ok();
        }
    }
}