using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.Entities;
using QuizApplication.Handlers;
using QuizApplication.ViewModels;

namespace QuizApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAuthHandler _authHandler;

        public AccountController(IAuthHandler authHandler)
        {
            _authHandler = authHandler;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var res = await _authHandler.SignIn(model.Username, model.Password);

            if (!res.Succeeded)
            {
                ModelState.AddModelError("LoginError", "Username or password is incorrect");
                return View(model);
            }

            return RedirectToAction("Instructions", "Quiz");
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var res = await _authHandler.Register(model.Username, model.Email,
                model.Password);
            
            if (res.Succeeded)
            {
                //add user to role
                await _authHandler.AddUserToRole(await _authHandler.GetUser(model.Username), AppUserRole.User);
                
                // login
                await _authHandler.SignIn(model.Username, model.Password);

                return RedirectToAction("Instructions", "Quiz");;
            };
            
            // add errors to the model
            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
                
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _authHandler.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}