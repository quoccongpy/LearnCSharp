using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Category;
using LearnCSharp.Application.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _serviceCategory;

        public CategoryController(ICategoryService serviceCategory)
        {
            _serviceCategory = serviceCategory;
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin)]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryListItemDTO>>> GetAllCategory()
        {
            var data = await _serviceCategory.GetAllCategoryAsync();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryListItemDTO>> GetByIdCategory(int id)
        {
            var data = await _serviceCategory.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
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