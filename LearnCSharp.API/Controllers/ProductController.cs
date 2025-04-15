using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Category;
using LearnCSharp.Application.Models.DTOs.Product;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _serviceProduct;

        public ProductController(IProductService serviceProduct)
        {
            _serviceProduct = serviceProduct;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProdcutPaging(string? keyword, int pageIndex=1, int pageSize = 10)
        {
            var data = await _serviceProduct.GetAllProductPagingAsync(keyword, pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("id")]
        public async Task<ActionResult<ProductDTO>> GetByIdProduct(int id)
        {
            var data = await _serviceProduct.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDTO model)
        {
            try
            {
                await _serviceProduct.CreateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDTO model)
        {
            try
            {
                await _serviceProduct.Update(id, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}