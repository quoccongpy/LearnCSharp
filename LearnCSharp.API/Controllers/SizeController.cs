using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Crust;
using LearnCSharp.Application.Models.DTOs.Size;
using LearnCSharp.Application.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/size")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> Create([FromBody] SizeDTO model)
        {
            await _sizeService.CreateAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SizeListItemDTO>>> GetAll()
        {
            var data = await _sizeService.GetAllSizeAsync();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SizeListItemDTO>> GetById(int id)
        {
            var data = await _sizeService.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SizeDTO model)
        {
            await _sizeService.Update(id, model);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sizeService.DeleteAsync(id);
            return Ok();
        }
    }
}