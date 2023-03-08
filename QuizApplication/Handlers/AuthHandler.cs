using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using QuizApplication.Entities;
using QuizApplication.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace QuizApplication.Handlers
{
    public interface IAuthHandler
    {
        Task<SignInResult> SignIn(string username, string password);
        Task<IdentityResult> Register(string username, string email, string password);
        Task AddUserToRole(AppUser user, AppUserRole userRole);
        Task<AppUser> GetUser(string userName);
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

        public async Task AddUserToRole(AppUser user, AppUserRole userRole)
        {
            await _userManager.AddToRoleAsync(user, userRole.ToString());
        }

        public async Task<AppUser> GetUser(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
    }
}