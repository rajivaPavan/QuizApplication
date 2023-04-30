using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using QuizApplication.Entities;
using QuizApplication.Models;
using QuizApplication.ViewModels.AccountViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace QuizApplication.Handlers
{
    public interface IAuthHandler
    {
        Task<SignInResult> SignIn(string username, string password);
        Task AddUserToRole(AppUser user, AppUserRole userRole);
        Task<AppUser> GetUser(string userName);
        Task SignOut();
        Task<IdentityResult> Register(RegisterViewModel username);
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
        
        public async Task<IdentityResult> Register(RegisterViewModel model)
        {
            var user = new AppUser
            {
                UserName = model.Username,
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task AddUserToRole(AppUser user, AppUserRole userRole)
        {
            await _userManager.AddToRoleAsync(user, userRole.ToString());
        }

        public async Task<AppUser> GetUser(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }
    }
}