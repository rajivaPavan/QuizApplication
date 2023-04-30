using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using QuizApplication.Entities;

namespace QuizApplication.Authorization
{
    public class QuizAccessAndTimeRequirement : IAuthorizationRequirement
    {
        public string RedirectUrl { get; }

        public QuizAccessAndTimeRequirement(string redirectUrl = "Error/AccessDenied")
        {
            RedirectUrl = redirectUrl;
        }
    }

    public class QuizAccessAndTimeHandler : AuthorizationHandler<QuizAccessAndTimeRequirement>
    {
        private readonly IFeatureManager _featureManager;
        private readonly HttpContext _httpContext;

        public QuizAccessAndTimeHandler(IFeatureManager featureManager, IHttpContextAccessor httpContextAccessor)
        {
            _featureManager = featureManager;
            _httpContext = httpContextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, QuizAccessAndTimeRequirement requirement)
        {
            var isAdmin = context.User.IsInRole(AppUserRoles.Admin);
            var quizAccess = await _featureManager.IsEnabledAsync(FeatureFlags.QuizAccess);
            var quizTime = await _featureManager.IsEnabledAsync(FeatureFlags.QuizTime);

            if (isAdmin || (quizAccess && quizTime))
            {
                context.Succeed(requirement);
            }
            else
            {
                var redirectUrl = $"{_httpContext.Request.PathBase}/{requirement.RedirectUrl}";
                _httpContext.Response.Redirect(redirectUrl);
                context.Fail();
            }
        }
    }

}