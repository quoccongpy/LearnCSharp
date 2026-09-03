using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Review;
using LearnCSharp.Application.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/product-reviews")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewService _reviewService;

        public ProductReviewController(IProductReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _reviewService.GetByProductIdAsync(productId, pageIndex, pageSize);
            return Ok(data);
        }
        [HttpGet("can-review")]
        [Authorize]
        public async Task<IActionResult> CanReview([FromQuery] int productId)
        {
            var canReview = await _reviewService.CanUserReviewAsync(productId);
            return Ok(new { canReview });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ProductReviewCreateDTO model)
        {
            await _reviewService.CreateAsync(model);
            return Ok();
        }
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ProductReviewUpdateDTO model)
        {
            await _reviewService.UpdateAsync(id, model);
            return Ok();
        }
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _reviewService.DeleteAsync(id);
            return Ok();
        }
        [HttpPatch("{id:int}/hide")]
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> Hide(int id, [FromQuery] bool isHidden)
        {
            await _reviewService.HideAsync(id, isHidden);
            return Ok();
        }
        [HttpGet("admin/all")]
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> GetAllForAdmin(int pageIndex = 1, int pageSize = 10, string? keyword = null)
        {
            var data = await _reviewService.GetAllForAdminAsync(pageIndex, pageSize, keyword);
            return Ok(data);
        }


    }
}
