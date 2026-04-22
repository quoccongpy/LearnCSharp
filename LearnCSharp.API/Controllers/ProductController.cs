using LearnCSharp.API.Mappers;
using LearnCSharp.API.Models;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Application.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _serviceProduct;

        public ProductController(IProductService serviceProduct)
        {
            _serviceProduct = serviceProduct;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductPaging(string? keyword, int? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _serviceProduct.GetAllProductPagingAsync(keyword, categoryId, pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetByIdProduct(int id)
        {
            var data = await _serviceProduct.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateRequest request)
        {
            var dto = ProductMapper.ToDTO(request);
            await _serviceProduct.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateRequest request)
        {
            var dto = ProductMapper.ToDTO(request);
            await _serviceProduct.Update(id, dto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _serviceProduct.DeleteAsync(id);
            return Ok();
        }
        [HttpGet("by-category/{categoryId:int}")]
        public async Task<ActionResult<List<ProductDTO>>> GetByCategory(int categoryId, int take = 50)
        {
            var data = await _serviceProduct.GetByCategoryAsync(categoryId, take);
            return Ok(data);
        }
    }
}