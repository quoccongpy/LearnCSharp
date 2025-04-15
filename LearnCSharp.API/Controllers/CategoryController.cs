using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Category;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _serviceCategory;

        public CategoryController(ICategoryService serviceCategory)
        {
            _serviceCategory = serviceCategory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO model)
        {
            try
            {
                await _serviceCategory.CreateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategory()
        {
            var data = await _serviceCategory.GetAllCategoryAsync();
            return Ok(data);
        }

        [HttpGet("id")]
        public async Task<ActionResult<CategoryDTO>> GetByIdCategory(int id)
        {
            var data = await _serviceCategory.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO model)
        {
            try
            {
                await _serviceCategory.Update(id, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _serviceCategory.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}