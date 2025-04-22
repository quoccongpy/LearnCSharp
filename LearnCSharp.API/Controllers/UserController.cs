using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO model)
        {
            try
            {
                await _userService.CreateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("id")]
        public async Task<ActionResult<UserDTO>> GetByIdUser(Guid id)
        {
            var data = await _userService.GetUserByIdAsync(id);
            return Ok(data);
        }
    }
}