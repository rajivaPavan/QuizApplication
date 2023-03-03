using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.Handlers;
using QuizApplication.RequestDTOs;

namespace QuizApplication.Controllers
{
    [Route("api/auth/[action]")]
    [ApiController]
    public class AuthApiController : Controller
    {
        private readonly IAuthHandler _authHandler;

        public AuthApiController(IAuthHandler authHandler)
        {
            _authHandler = authHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var res = await _authHandler.SignIn(loginDto.Username, loginDto.Password);

            if (!res.Succeeded)
            {
                return BadRequest(new {message = "Invalid username or password"});
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var res = await _authHandler.Register(registerDto.Username, registerDto.Email,
                registerDto.Password);

            if (res.Succeeded)
            {
                // login
                await _authHandler.SignIn(registerDto.Username, registerDto.Password);

                return Redirect(Routes.Instructions);
            };
            
            // add errors to the model
            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
                
            return BadRequest("Error while registering");

            //send redirect url to frontend to redirect to login page


        }

    }
}