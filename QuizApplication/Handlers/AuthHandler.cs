using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizApplication.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace QuizApplication.Handlers
{
    public interface IAuthHandler
    {
        Task<SignInResult> SignIn(string username, string password);
        Task<IdentityResult> Register(string username, string email, string password);
    }
    
    public class AuthHandler : IAuthHandler
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> SignIn(string username, string password)
        {
            var result = await _signInManager
                .PasswordSignInAsync(username, password, false, false);
            return result;
        }

        public async Task<IdentityResult> Register(string username, string email, string password)
        {
            var user = new AppUser()
            {
                UserName = username, Email = email
            };
            
            var result = await _userManager.CreateAsync(user, password);

            return result;
        }
    }
}