using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Crust;
using LearnCSharp.Application.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/crust")]
    [ApiController]
    public class CrustController : ControllerBase
    {
        private readonly ICrustService _crustService;

        public CrustController(ICrustService crustService)
        {
            _crustService = crustService;
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> Create([FromBody] CrustDTO model)
        {
            await _crustService.CreateAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CrustListItemDTO>>> GetAll()
        {
            var data = await _crustService.GetAllCrustAsync();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CrustListItemDTO>> GetById(int id)
        {
            var data = await _crustService.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrustDTO model)
        {
            await _crustService.Update(id, model);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _crustService.DeleteAsync(id);
            return Ok();
        }
    }
}