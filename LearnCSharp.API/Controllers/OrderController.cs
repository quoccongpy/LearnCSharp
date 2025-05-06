using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Order;
using LearnCSharp.Application.Models.DTOs.Product;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] OrderCreateDTO model)
        {
            try
            {
                await _orderService.CreateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrderPaging(string? keyword, string? status, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _orderService.GetAllOrderPagingAsync(keyword, status, pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("id")]
        public async Task<ActionResult<ProductDTO>> GetByIdOrder(int id)
        {
            var data = await _orderService.GetOrderByIdAsync(id);
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
    }
}