using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransportationApplication.UserService.DTOs;
using TransportationApplication.UserService.Services;

namespace TransportationApplication.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //injections
        private readonly IUserServices _userServices;

        //constructors
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("login")]
        public async Task <IActionResult> Login([FromBody]LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _userServices.LoginUserAsync(loginViewModel);

            if (token == null)
            {
                return Unauthorized("Invalid login.");
            }

            return Ok(new { Token = token });
        }
    }
}
