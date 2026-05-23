using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Application.Models.DTOs.ProductVariant;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/product-variant")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductVariantCreateDTO model)
        {
            await _productVariantService.CreateAsync(model);
            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductVariantListItemDTO>> GetById(int id)
        {
            var data = await _productVariantService.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductVariantUpdateDTO model)
        {
            await _productVariantService.Update(id, model);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productVariantService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("product-with-variant")]
        public async Task<ActionResult<IEnumerable<ProductSimpleDTO>>> GetProductsWithVariant()
        {
            var data = await _productVariantService.GetProductsWithVariantAsync();
            return Ok(data);
        }

        [HttpGet("by-product/{productId:int}")]
        public async Task<ActionResult<IEnumerable<ProductVariantListItemDTO>>> GetByProductIdAsync(int productId)
        {
            var data = await _productVariantService.GetByProductIdAsync(productId);
            return Ok(data);
        }
    }
}