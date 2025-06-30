using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]  
        public async Task<ActionResult<AuthDTO>> Login([FromBody] LoginDTO model)
        {
            try
            {
                var data = await _authService.LoginAsync(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthDTO>> RefreshToken([FromBody] RefreshTokenDTO model)
        {
            try
            {
                var data = await _authService.RefreshTokenAsync(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
