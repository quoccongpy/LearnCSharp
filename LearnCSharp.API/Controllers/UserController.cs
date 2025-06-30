using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/user")]
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

        [HttpPut("id")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO model)
        {
            try
            {
                await _userService.UpdateAsync(id, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> UpdateUserActiveStatus(Guid id, [FromBody] bool isActive)
        {
            try
            {
                await _userService.DeleteAsync(id, isActive);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO model)
        {
            try
            {
                await _userService.ChangePasswordAsync(id, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}